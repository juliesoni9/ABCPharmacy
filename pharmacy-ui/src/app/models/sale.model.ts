export interface SaleRecord {
  id: string;
  medicineId: string;
  medicineName: string;
  quantitySold: number;
  unitPrice: number;
  totalPrice: number;
  saleDate: string;
}

export interface CreateSaleRequest {
  medicineId: string;
  quantitySold: number;
}
