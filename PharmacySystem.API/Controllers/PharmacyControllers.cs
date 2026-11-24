using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;

    public MedicinesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _medicineService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _medicineService.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
        => Ok(await _medicineService.SearchAsync(query));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMedicineDto dto)
        => Ok(await _medicineService.CreateAsync(dto));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicineDto dto)
        => Ok(await _medicineService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _medicineService.DeleteAsync(id);
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _customerService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _customerService.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        => Ok(await _customerService.CreateAsync(dto));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
        => Ok(await _customerService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _customerService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:int}/pay-debt")]
    public async Task<IActionResult> PayDebt(int id, [FromQuery] decimal amount, [FromQuery] string note = "")
    {
        await _customerService.RecordDebtPaymentAsync(id, amount, note);
        return Ok(new { message = "تم تسجيل سداد الدفعة بنجاح" });
    }

    [HttpGet("{id:int}/payments")]
    public async Task<IActionResult> GetPayments(int id)
        => Ok(await _customerService.GetDebtPaymentsAsync(id));
}

public record CreateInvoiceRequest(
    string InvoiceNumber,
    int CustomerId,
    decimal PaidAmount,
    string? Note,
    List<SaleItemRequestDto> Items
);

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public InvoicesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _saleService.GetAllInvoicesAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _saleService.GetInvoiceByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request)
    {
        var dto = new CreateSaleInvoiceDto(
            request.InvoiceNumber,
            request.CustomerId,
            request.PaidAmount,
            request.Note ?? string.Empty,
            request.Items
        );

        var created = await _saleService.CreateInvoiceAsync(dto);
        return Ok(created);
    }
}