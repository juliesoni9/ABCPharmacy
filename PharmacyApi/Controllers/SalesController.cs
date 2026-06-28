using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Models;
using PharmacyApi.Services;

namespace PharmacyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SaleRecord>>> GetAll()
    {
        var sales = await _saleService.GetAllAsync();
        return Ok(sales);
    }

    [HttpPost]
    public async Task<ActionResult<SaleRecord>> RecordSale([FromBody] CreateSaleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (sale, error) = await _saleService.RecordSaleAsync(request);
        if (sale is null)
        {
            return BadRequest(new { message = error });
        }

        return CreatedAtAction(nameof(GetAll), sale);
    }
}
