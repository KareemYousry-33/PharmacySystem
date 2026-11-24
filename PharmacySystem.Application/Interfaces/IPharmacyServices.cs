using PharmacySystem.Application.DTOs;

namespace PharmacySystem.Application.Interfaces;

public interface IMedicineService
{
    Task<MedicineDto> CreateAsync(CreateMedicineDto dto);
    Task<List<MedicineDto>> GetAllAsync();
    Task<MedicineDto?> GetByIdAsync(int id);
    Task<List<MedicineDto>> SearchAsync(string query);
    Task<MedicineDto> UpdateAsync(int id, UpdateMedicineDto dto);
    Task DeleteAsync(int id);
}

public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<List<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto);
    Task DeleteAsync(int id);
    Task RecordDebtPaymentAsync(int customerId, decimal amount, string note);
    Task<List<DebtPaymentDto>> GetDebtPaymentsAsync(int customerId);
}

public interface ISaleService
{
    Task<SaleInvoiceDto> CreateInvoiceAsync(CreateSaleInvoiceDto dto);
    Task<List<SaleInvoiceDto>> GetAllInvoicesAsync();
    Task<SaleInvoiceDetailsDto?> GetInvoiceByIdAsync(int id);
}