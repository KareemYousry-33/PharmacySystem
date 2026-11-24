using Microsoft.EntityFrameworkCore;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Infrastructure.Data;

namespace PharmacySystem.Infrastructure.Services;

public class MedicineService : IMedicineService
{
    private readonly PharmacyDbContext _context;

    public MedicineService(PharmacyDbContext context)
    {
        _context = context;
    }

    public async Task<MedicineDto> CreateAsync(CreateMedicineDto dto)
    {
        var medicine = new Medicine
        {
            Name = dto.Name,
            Code = dto.Code,
            Price = dto.Price,
            Quantity = dto.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        _context.Medicines.Add(medicine);
        await _context.SaveChangesAsync();

        return new MedicineDto(medicine.Id, medicine.Name, medicine.Code, medicine.Price, medicine.Quantity);
    }

    public async Task<List<MedicineDto>> GetAllAsync()
    {
        return await _context.Medicines
            .Select(m => new MedicineDto(m.Id, m.Name, m.Code, m.Price, m.Quantity))
            .ToListAsync();
    }

    public async Task<MedicineDto?> GetByIdAsync(int id)
    {
        var medicine = await _context.Medicines.FindAsync(id);
        return medicine is null
            ? null
            : new MedicineDto(medicine.Id, medicine.Name, medicine.Code, medicine.Price, medicine.Quantity);
    }

    public async Task<List<MedicineDto>> SearchAsync(string query)
    {
        var q = query.Trim().ToLower();
        return await _context.Medicines
            .Where(m => m.Name.ToLower().Contains(q) || m.Code.ToLower().Contains(q))
            .Select(m => new MedicineDto(m.Id, m.Name, m.Code, m.Price, m.Quantity))
            .ToListAsync();
    }

    public async Task<MedicineDto> UpdateAsync(int id, UpdateMedicineDto dto)
    {
        var medicine = await _context.Medicines.FindAsync(id)
            ?? throw new KeyNotFoundException("الدواء غير موجود");

        medicine.Name = dto.Name;
        medicine.Code = dto.Code;
        medicine.Price = dto.Price;
        medicine.Quantity = dto.Quantity;

        await _context.SaveChangesAsync();
        return new MedicineDto(medicine.Id, medicine.Name, medicine.Code, medicine.Price, medicine.Quantity);
    }

    public async Task DeleteAsync(int id)
    {
        var medicine = await _context.Medicines.FindAsync(id)
            ?? throw new KeyNotFoundException("الدواء غير موجود");

        _context.Medicines.Remove(medicine);
        await _context.SaveChangesAsync();
    }
}

public class CustomerService : ICustomerService
{
    private readonly PharmacyDbContext _context;

    public CustomerService(PharmacyDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            TotalDebt = 0
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return new CustomerDto(customer.Id, customer.Name, customer.Address, customer.PhoneNumber, customer.TotalDebt);
    }

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        return await _context.Customers
            .Select(c => new CustomerDto(c.Id, c.Name, c.Address, c.PhoneNumber, c.TotalDebt))
            .ToListAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        return customer is null
            ? null
            : new CustomerDto(customer.Id, customer.Name, customer.Address, customer.PhoneNumber, customer.TotalDebt);
    }

    public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id)
            ?? throw new KeyNotFoundException("العميل غير موجود");

        customer.Name = dto.Name;
        customer.Address = dto.Address;
        customer.PhoneNumber = dto.PhoneNumber;

        await _context.SaveChangesAsync();
        return new CustomerDto(customer.Id, customer.Name, customer.Address, customer.PhoneNumber, customer.TotalDebt);
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id)
            ?? throw new KeyNotFoundException("العميل غير موجود");

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }

    public async Task RecordDebtPaymentAsync(int customerId, decimal amount, string note)
    {
        if (amount <= 0)
            throw new InvalidOperationException("قيمة الدفعة يجب أن تكون أكبر من صفر");

        var customer = await _context.Customers.FindAsync(customerId)
            ?? throw new KeyNotFoundException("العميل غير موجود");

        var payment = new DebtPayment
        {
            CustomerId = customerId,
            Amount = amount,
            PaymentDate = DateTime.UtcNow,
            Note = note
        };

        customer.TotalDebt -= amount;
        if (customer.TotalDebt < 0) customer.TotalDebt = 0;

        _context.DebtPayments.Add(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<List<DebtPaymentDto>> GetDebtPaymentsAsync(int customerId)
    {
        return await _context.DebtPayments
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new DebtPaymentDto(p.Id, p.Amount, p.PaymentDate, p.Note))
            .ToListAsync();
    }
}

public class SaleService : ISaleService
{
    private readonly PharmacyDbContext _context;

    public SaleService(PharmacyDbContext context)
    {
        _context = context;
    }

    public async Task<SaleInvoiceDto> CreateInvoiceAsync(CreateSaleInvoiceDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var customer = await _context.Customers.FindAsync(dto.CustomerId)
            ?? throw new KeyNotFoundException("العميل غير موجود");

        decimal totalAmount = 0;
        var saleItems = new List<SaleItem>();

        foreach (var itemDto in dto.Items)
        {
            var medicine = await _context.Medicines.FindAsync(itemDto.MedicineId)
                ?? throw new KeyNotFoundException($"الدواء رقم {itemDto.MedicineId} غير موجود");

            if (medicine.Quantity < itemDto.Quantity)
                throw new InvalidOperationException($"الكمية غير كافية للدواء: {medicine.Name}");

            medicine.Quantity -= itemDto.Quantity;
            var lineTotal = medicine.Price * itemDto.Quantity;
            totalAmount += lineTotal;

            saleItems.Add(new SaleItem
            {
                MedicineId = medicine.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = medicine.Price
            });
        }

        var remainingAmount = totalAmount - dto.PaidAmount;
        if (remainingAmount < 0) remainingAmount = 0;

        var invoice = new SaleInvoice
        {
            InvoiceNumber = dto.InvoiceNumber,
            CustomerId = dto.CustomerId,
            CreatedAtDateTime = DateTime.UtcNow,
            TotalAmount = totalAmount,
            PaidAmount = dto.PaidAmount,
            RemainingAmount = remainingAmount,
            Note = dto.Note,
            SaleItems = saleItems
        };

        customer.TotalDebt += remainingAmount;

        _context.SaleInvoices.Add(invoice);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new SaleInvoiceDto(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.CustomerId,
            invoice.CreatedAtDateTime,
            invoice.TotalAmount,
            invoice.PaidAmount,
            invoice.RemainingAmount
        );
    }

    public async Task<List<SaleInvoiceDto>> GetAllInvoicesAsync()
    {
        return await _context.SaleInvoices
            .Select(i => new SaleInvoiceDto(
                i.Id,
                i.InvoiceNumber,
                i.CustomerId,
                i.CreatedAtDateTime,
                i.TotalAmount,
                i.PaidAmount,
                i.RemainingAmount
            ))
            .ToListAsync();
    }

    public async Task<SaleInvoiceDetailsDto?> GetInvoiceByIdAsync(int id)
    {
        var invoice = await _context.SaleInvoices
            .Include(i => i.SaleItems)
                .ThenInclude(item => item.Medicine)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice is null) return null;

        var customer = await _context.Customers.FindAsync(invoice.CustomerId);

        var items = invoice.SaleItems.Select(item => new SaleItemDetailDto(
            item.MedicineId,
            item.Medicine?.Name ?? $"Medicine #{item.MedicineId}",
            item.Quantity,
            item.UnitPrice,
            item.UnitPrice * item.Quantity
        )).ToList();

        return new SaleInvoiceDetailsDto(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.CustomerId,
            customer?.Name ?? $"Customer #{invoice.CustomerId}",
            invoice.CreatedAtDateTime,
            invoice.TotalAmount,
            invoice.PaidAmount,
            invoice.RemainingAmount,
            invoice.Note,
            items
        );
    }
}