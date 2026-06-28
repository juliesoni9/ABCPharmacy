using PharmacyApi.Models;

namespace PharmacyApi.Services;

public interface ISaleService
{
    Task<List<SaleRecord>> GetAllAsync();
    Task<(SaleRecord? Sale, string? Error)> RecordSaleAsync(CreateSaleRequest request);
}
