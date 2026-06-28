using PharmacyApi.Models;
using PharmacyApi.Services;
using PharmacyApi.Tests.Helpers;

namespace PharmacyApi.Tests.Services;

public class SaleServiceTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly MedicineService _medicineService;
    private readonly SaleService _saleService;

    public SaleServiceTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), "PharmacyApiTests", Guid.NewGuid().ToString());
        var environment = TestEnvironmentFactory.Create(_tempRoot);
        var medicineStore = new JsonFileStore<Medicine>(environment, "medicines.json");
        var saleStore = new JsonFileStore<SaleRecord>(environment, "sales.json");
        _medicineService = new MedicineService(medicineStore);
        _saleService = new SaleService(saleStore, _medicineService);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    [Fact]
    public async Task RecordSaleAsync_CreatesSaleAndReducesStock()
    {
        var (medicine, _) = await _medicineService.CreateAsync(
            TestData.CreateMedicineRequest(quantity: 10, price: 5.00m));

        var (sale, error) = await _saleService.RecordSaleAsync(new CreateSaleRequest
        {
            MedicineId = medicine!.Id,
            QuantitySold = 3
        });

        var updatedMedicine = await _medicineService.GetByIdAsync(medicine.Id);

        Assert.Null(error);
        Assert.NotNull(sale);
        Assert.Equal(3, sale.QuantitySold);
        Assert.Equal(15.00m, sale.TotalPrice);
        Assert.Equal(7, updatedMedicine!.Quantity);
    }

    [Fact]
    public async Task RecordSaleAsync_ReturnsError_WhenQuantityIsZero()
    {
        var (medicine, _) = await _medicineService.CreateAsync(TestData.CreateMedicineRequest());

        var (sale, error) = await _saleService.RecordSaleAsync(new CreateSaleRequest
        {
            MedicineId = medicine!.Id,
            QuantitySold = 0
        });

        Assert.Null(sale);
        Assert.Equal("Quantity sold must be greater than zero.", error);
    }

    [Fact]
    public async Task RecordSaleAsync_ReturnsError_WhenMedicineNotFound()
    {
        var (sale, error) = await _saleService.RecordSaleAsync(new CreateSaleRequest
        {
            MedicineId = Guid.NewGuid(),
            QuantitySold = 1
        });

        Assert.Null(sale);
        Assert.Equal("Medicine not found.", error);
    }

    [Fact]
    public async Task RecordSaleAsync_ReturnsError_WhenInsufficientStock()
    {
        var (medicine, _) = await _medicineService.CreateAsync(
            TestData.CreateMedicineRequest(quantity: 2));

        var (sale, error) = await _saleService.RecordSaleAsync(new CreateSaleRequest
        {
            MedicineId = medicine!.Id,
            QuantitySold = 5
        });

        Assert.Null(sale);
        Assert.Contains("Insufficient stock", error);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSalesOrderedByDateDescending()
    {
        var (medicine, _) = await _medicineService.CreateAsync(
            TestData.CreateMedicineRequest(quantity: 100));

        await _saleService.RecordSaleAsync(new CreateSaleRequest
        {
            MedicineId = medicine!.Id,
            QuantitySold = 1
        });

        await Task.Delay(10);

        await _saleService.RecordSaleAsync(new CreateSaleRequest
        {
            MedicineId = medicine.Id,
            QuantitySold = 2
        });

        var sales = await _saleService.GetAllAsync();

        Assert.Equal(2, sales.Count);
        Assert.True(sales[0].SaleDate >= sales[1].SaleDate);
    }
}
