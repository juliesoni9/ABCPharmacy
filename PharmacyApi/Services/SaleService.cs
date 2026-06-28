using PharmacyApi.Models;

namespace PharmacyApi.Services;

public class SaleService : ISaleService
{
    private readonly JsonFileStore<SaleRecord> _store;
    private readonly IMedicineService _medicineService;

    public SaleService(JsonFileStore<SaleRecord> store, IMedicineService medicineService)
    {
        _store = store;
        _medicineService = medicineService;
    }

    public async Task<List<SaleRecord>> GetAllAsync()
    {
        var sales = await _store.ReadAllAsync();
        return sales.OrderByDescending(s => s.SaleDate).ToList();
    }

    public async Task<(SaleRecord? Sale, string? Error)> RecordSaleAsync(CreateSaleRequest request)
    {
        if (request.QuantitySold <= 0)
        {
            return (null, "Quantity sold must be greater than zero.");
        }

        var medicine = await _medicineService.GetByIdAsync(request.MedicineId);
        if (medicine is null)
        {
            return (null, "Medicine not found.");
        }

        if (medicine.Quantity < request.QuantitySold)
        {
            return (null, $"Insufficient stock. Only {medicine.Quantity} units available.");
        }

        var sale = new SaleRecord
        {
            Id = Guid.NewGuid(),
            MedicineId = medicine.Id,
            MedicineName = medicine.FullName,
            QuantitySold = request.QuantitySold,
            UnitPrice = medicine.Price,
            TotalPrice = Math.Round(medicine.Price * request.QuantitySold, 2),
            SaleDate = DateTime.UtcNow
        };

        var updated = await _medicineService.UpdateQuantityAsync(
            medicine.Id,
            medicine.Quantity - request.QuantitySold);

        if (!updated)
        {
            return (null, "Failed to update medicine stock.");
        }

        var sales = await _store.ReadAllAsync();
        sales.Add(sale);
        await _store.WriteAllAsync(sales);

        return (sale, null);
    }
}
