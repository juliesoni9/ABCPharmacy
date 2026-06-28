using Microsoft.AspNetCore.Mvc;
using Moq;
using PharmacyApi.Controllers;
using PharmacyApi.Models;
using PharmacyApi.Services;
using PharmacyApi.Tests.Helpers;

namespace PharmacyApi.Tests.Controllers;

public class SalesControllerTests
{
    private readonly Mock<ISaleService> _saleServiceMock;
    private readonly SalesController _controller;

    public SalesControllerTests()
    {
        _saleServiceMock = new Mock<ISaleService>();
        _controller = new SalesController(_saleServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithSales()
    {
        var sales = new List<SaleRecord>
        {
            new()
            {
                Id = Guid.NewGuid(),
                MedicineId = Guid.NewGuid(),
                MedicineName = "Paracetamol 500mg",
                QuantitySold = 2,
                UnitPrice = 12.50m,
                TotalPrice = 25.00m,
                SaleDate = DateTime.UtcNow
            }
        };

        _saleServiceMock
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(sales);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<List<SaleRecord>>(okResult.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task RecordSale_ReturnsCreated_WhenSaleSucceeds()
    {
        var medicineId = Guid.NewGuid();
        var request = new CreateSaleRequest { MedicineId = medicineId, QuantitySold = 2 };
        var sale = new SaleRecord
        {
            Id = Guid.NewGuid(),
            MedicineId = medicineId,
            MedicineName = "Paracetamol 500mg",
            QuantitySold = 2,
            UnitPrice = 12.50m,
            TotalPrice = 25.00m,
            SaleDate = DateTime.UtcNow
        };

        _saleServiceMock
            .Setup(s => s.RecordSaleAsync(request))
            .ReturnsAsync((sale, null));

        var result = await _controller.RecordSale(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(sale, createdResult.Value);
    }

    [Fact]
    public async Task RecordSale_ReturnsBadRequest_WhenServiceReturnsError()
    {
        var request = new CreateSaleRequest { MedicineId = Guid.NewGuid(), QuantitySold = 10 };
        _saleServiceMock
            .Setup(s => s.RecordSaleAsync(request))
            .ReturnsAsync((null, "Insufficient stock. Only 2 units available."));

        var result = await _controller.RecordSale(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
