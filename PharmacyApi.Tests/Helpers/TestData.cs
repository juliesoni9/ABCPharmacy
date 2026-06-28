using PharmacyApi.Models;

namespace PharmacyApi.Tests.Helpers;

public static class TestData
{
    public static CreateMedicineRequest CreateMedicineRequest(
        string fullName = "Paracetamol 500mg",
        string brand = "Tylenol",
        int quantity = 50,
        decimal price = 12.50m,
        DateOnly? expiryDate = null,
        string notes = "Test notes")
    {
        return new CreateMedicineRequest
        {
            FullName = fullName,
            Brand = brand,
            Quantity = quantity,
            Price = price,
            ExpiryDate = expiryDate ?? new DateOnly(2027, 6, 1),
            Notes = notes
        };
    }

    public static Medicine CreateMedicine(
        Guid? id = null,
        string fullName = "Paracetamol 500mg",
        string brand = "Tylenol",
        int quantity = 50,
        decimal price = 12.50m)
    {
        return new Medicine
        {
            Id = id ?? Guid.NewGuid(),
            FullName = fullName,
            Brand = brand,
            Quantity = quantity,
            Price = price,
            ExpiryDate = new DateOnly(2027, 6, 1),
            Notes = string.Empty
        };
    }
}
