namespace PharmacySystem.Application.DTOs;

public record CreateMedicineDto(string Name, string Code, decimal Price, int Quantity);
public record UpdateMedicineDto(string Name, string Code, decimal Price, int Quantity);
public record MedicineDto(int Id, string Name, string Code, decimal Price, int Quantity);

public record CreateCustomerDto(string Name, string Address, string PhoneNumber);
public record UpdateCustomerDto(string Name, string Address, string PhoneNumber);
public record CustomerDto(int Id, string Name, string Address, string PhoneNumber, decimal TotalDebt);
public record DebtPaymentDto(int Id, decimal Amount, DateTime PaymentDate, string Note);

public record SaleItemRequestDto(int MedicineId, int Quantity);
public record SaleItemDetailDto(int MedicineId, string MedicineName, int Quantity, decimal UnitPrice, decimal LineTotal);
public record CreateSaleInvoiceDto(string InvoiceNumber, int CustomerId, decimal PaidAmount, string Note, List<SaleItemRequestDto> Items);
public record SaleInvoiceDto(int Id, string InvoiceNumber, int CustomerId, DateTime CreatedAtDateTime, decimal TotalAmount, decimal PaidAmount, decimal RemainingAmount);
public record SaleInvoiceDetailsDto(
    int Id,
    string InvoiceNumber,
    int CustomerId,
    string CustomerName,
    DateTime CreatedAtDateTime,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    string Note,
    List<SaleItemDetailDto> Items
);