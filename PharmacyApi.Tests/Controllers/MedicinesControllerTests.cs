using Microsoft.AspNetCore.Mvc;
using Moq;
using PharmacyApi.Controllers;
using PharmacyApi.Models;
using PharmacyApi.Services;
using PharmacyApi.Tests.Helpers;

namespace PharmacyApi.Tests.Controllers;

public class MedicinesControllerTests
{
    private readonly Mock<IMedicineService> _medicineServiceMock;
    private readonly MedicinesController _controller;

    public MedicinesControllerTests()
    {
        _medicineServiceMock = new Mock<IMedicineService>();
        _controller = new MedicinesController(_medicineServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithMedicines()
    {
        var medicines = new List<Medicine> { TestData.CreateMedicine() };
        _medicineServiceMock
            .Setup(s => s.GetAllAsync(null))
            .ReturnsAsync(medicines);

        var result = await _controller.GetAll(null);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<List<Medicine>>(okResult.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task GetAll_PassesSearchTermToService()
    {
        _medicineServiceMock
            .Setup(s => s.GetAllAsync("aspirin"))
            .ReturnsAsync([]);

        await _controller.GetAll("aspirin");

        _medicineServiceMock.Verify(s => s.GetAllAsync("aspirin"), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMedicineMissing()
    {
        var id = Guid.NewGuid();
        _medicineServiceMock
            .Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync((Medicine?)null);

        var result = await _controller.GetById(id);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenMedicineExists()
    {
        var medicine = TestData.CreateMedicine();
        _medicineServiceMock
            .Setup(s => s.GetByIdAsync(medicine.Id))
            .ReturnsAsync(medicine);

        var result = await _controller.GetById(medicine.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(medicine, okResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenMedicineIsAdded()
    {
        var request = TestData.CreateMedicineRequest();
        var medicine = TestData.CreateMedicine(fullName: request.FullName, brand: request.Brand);
        _medicineServiceMock
            .Setup(s => s.CreateAsync(request))
            .ReturnsAsync((medicine, null));

        var result = await _controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(medicine, createdResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenDuplicateExists()
    {
        var request = TestData.CreateMedicineRequest();
        _medicineServiceMock
            .Setup(s => s.CreateAsync(request))
            .ReturnsAsync((null, "A medicine named \"Paracetamol 500mg\" with brand \"Tylenol\" already exists."));

        var result = await _controller.Create(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badRequest.Value);
    }
}
