namespace PharmacyApi.Models;

public class SaleRecord
{
    public Guid Id { get; set; }
    public Guid MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime SaleDate { get; set; }
}

public class CreateSaleRequest
{
    public Guid MedicineId { get; set; }
    public int QuantitySold { get; set; }
}
