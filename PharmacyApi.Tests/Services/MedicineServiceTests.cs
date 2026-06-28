using PharmacyApi.Models;
using PharmacyApi.Services;
using PharmacyApi.Tests.Helpers;

namespace PharmacyApi.Tests.Services;

public class MedicineServiceTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly MedicineService _service;

    public MedicineServiceTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), "PharmacyApiTests", Guid.NewGuid().ToString());
        var environment = TestEnvironmentFactory.Create(_tempRoot);
        var store = new JsonFileStore<Medicine>(environment, "medicines.json");
        _service = new MedicineService(store);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    [Fact]
    public async Task CreateAsync_ReturnsMedicine_WhenRequestIsValid()
    {
        var request = TestData.CreateMedicineRequest();

        var (medicine, error) = await _service.CreateAsync(request);

        Assert.Null(error);
        Assert.NotNull(medicine);
        Assert.Equal("Paracetamol 500mg", medicine.FullName);
        Assert.Equal("Tylenol", medicine.Brand);
        Assert.Equal(50, medicine.Quantity);
        Assert.Equal(12.50m, medicine.Price);
    }

    [Fact]
    public async Task CreateAsync_ReturnsError_WhenNameAndBrandDuplicate()
    {
        var request = TestData.CreateMedicineRequest();

        await _service.CreateAsync(request);
        var (medicine, error) = await _service.CreateAsync(request);

        Assert.Null(medicine);
        Assert.Contains("already exists", error);
    }

    [Fact]
    public async Task CreateAsync_AllowsSameName_WithDifferentBrand()
    {
        await _service.CreateAsync(TestData.CreateMedicineRequest(brand: "Tylenol"));
        var (medicine, error) = await _service.CreateAsync(
            TestData.CreateMedicineRequest(brand: "Generic"));

        Assert.Null(error);
        Assert.NotNull(medicine);
    }

    [Fact]
    public async Task CreateAsync_TreatsDuplicateAsCaseInsensitive()
    {
        await _service.CreateAsync(TestData.CreateMedicineRequest(fullName: "Paracetamol 500mg", brand: "Tylenol"));
        var (medicine, error) = await _service.CreateAsync(
            TestData.CreateMedicineRequest(fullName: "paracetamol 500mg", brand: "tylenol"));

        Assert.Null(medicine);
        Assert.NotNull(error);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMedicinesSortedByName()
    {
        await _service.CreateAsync(TestData.CreateMedicineRequest(fullName: "Zinc Tablets"));
        await _service.CreateAsync(TestData.CreateMedicineRequest(fullName: "Aspirin"));

        var medicines = await _service.GetAllAsync();

        Assert.Equal(2, medicines.Count);
        Assert.Equal("Aspirin", medicines[0].FullName);
        Assert.Equal("Zinc Tablets", medicines[1].FullName);
    }

    [Fact]
    public async Task GetAllAsync_FiltersBySearchTerm()
    {
        await _service.CreateAsync(TestData.CreateMedicineRequest(fullName: "Paracetamol 500mg"));
        await _service.CreateAsync(TestData.CreateMedicineRequest(fullName: "Ibuprofen 200mg"));

        var medicines = await _service.GetAllAsync("para");

        Assert.Single(medicines);
        Assert.Equal("Paracetamol 500mg", medicines[0].FullName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMedicine_WhenExists()
    {
        var (created, _) = await _service.CreateAsync(TestData.CreateMedicineRequest());

        var medicine = await _service.GetByIdAsync(created!.Id);

        Assert.NotNull(medicine);
        Assert.Equal(created.Id, medicine.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var medicine = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(medicine);
    }

    [Fact]
    public async Task UpdateQuantityAsync_UpdatesStock_WhenMedicineExists()
    {
        var (created, _) = await _service.CreateAsync(TestData.CreateMedicineRequest(quantity: 20));

        var updated = await _service.UpdateQuantityAsync(created!.Id, 15);
        var medicine = await _service.GetByIdAsync(created.Id);

        Assert.True(updated);
        Assert.Equal(15, medicine!.Quantity);
    }

    [Fact]
    public async Task UpdateQuantityAsync_ReturnsFalse_WhenMedicineNotFound()
    {
        var updated = await _service.UpdateQuantityAsync(Guid.NewGuid(), 10);

        Assert.False(updated);
    }
}
