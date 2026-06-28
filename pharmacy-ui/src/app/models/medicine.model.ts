export interface Medicine {
  id: string;
  fullName: string;
  notes: string;
  expiryDate: string;
  quantity: number;
  price: number;
  brand: string;
}

export interface CreateMedicineRequest {
  fullName: string;
  notes: string;
  expiryDate: string;
  quantity: number;
  price: number;
  brand: string;
}
