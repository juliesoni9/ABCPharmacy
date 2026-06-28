using PharmacyApi.Models;

namespace PharmacyApi.Services;

public class MedicineService : IMedicineService
{
    private readonly JsonFileStore<Medicine> _store;

    public MedicineService(JsonFileStore<Medicine> store)
    {
        _store = store;
    }

    public async Task<List<Medicine>> GetAllAsync(string? search = null)
    {
        var medicines = await _store.ReadAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            medicines = medicines
                .Where(m => m.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return medicines.OrderBy(m => m.FullName).ToList();
    }

    public async Task<Medicine?> GetByIdAsync(Guid id)
    {
        var medicines = await _store.ReadAllAsync();
        return medicines.FirstOrDefault(m => m.Id == id);
    }

    public async Task<(Medicine? Medicine, string? Error)> CreateAsync(CreateMedicineRequest request)
    {
        var fullName = request.FullName.Trim();
        var brand = request.Brand.Trim();
        var medicines = await _store.ReadAllAsync();

        if (IsDuplicate(medicines, fullName, brand))
        {
            return (null, $"A medicine named \"{fullName}\" with brand \"{brand}\" already exists.");
        }

        var medicine = new Medicine
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Notes = request.Notes.Trim(),
            ExpiryDate = request.ExpiryDate,
            Quantity = request.Quantity,
            Price = Math.Round(request.Price, 2),
            Brand = brand
        };

        medicines.Add(medicine);
        await _store.WriteAllAsync(medicines);
        return (medicine, null);
    }

    private static bool IsDuplicate(List<Medicine> medicines, string fullName, string brand)
    {
        return medicines.Any(m =>
            m.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase) &&
            m.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> UpdateQuantityAsync(Guid id, int newQuantity)
    {
        var medicines = await _store.ReadAllAsync();
        var medicine = medicines.FirstOrDefault(m => m.Id == id);
        if (medicine is null)
        {
            return false;
        }

        medicine.Quantity = newQuantity;
        await _store.WriteAllAsync(medicines);
        return true;
    }
}
