using PharmacyApi.Models;

namespace PharmacyApi.Services;

public interface IMedicineService
{
    Task<List<Medicine>> GetAllAsync(string? search = null);
    Task<Medicine?> GetByIdAsync(Guid id);
    Task<(Medicine? Medicine, string? Error)> CreateAsync(CreateMedicineRequest request);
    Task<bool> UpdateQuantityAsync(Guid id, int newQuantity);
}
