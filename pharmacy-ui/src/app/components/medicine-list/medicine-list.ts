import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MedicineService } from '../../services/medicine.service';
import { SaleService } from '../../services/sale.service';
import { Medicine } from '../../models/medicine.model';
import { MedicineFormComponent } from '../medicine-form/medicine-form';

type RowStatus = 'expiring' | 'low-stock' | 'normal';

@Component({
  selector: 'app-medicine-list',
  imports: [CommonModule, FormsModule, MedicineFormComponent],
  templateUrl: './medicine-list.html',
  styleUrl: './medicine-list.scss'
})
export class MedicineListComponent implements OnInit {
  medicines = signal<Medicine[]>([]);
  searchTerm = signal('');
  loading = signal(false);
  error = signal<string | null>(null);
  showAddForm = signal(false);
  saleMedicineId = signal<string | null>(null);
  saleQuantity = signal(1);
  saleError = signal<string | null>(null);
  saleSuccess = signal<string | null>(null);

  constructor(
    private medicineService: MedicineService,
    private saleService: SaleService
  ) {}

  ngOnInit(): void {
    this.loadMedicines();
  }

  loadMedicines(): void {
    this.loading.set(true);
    this.error.set(null);
    this.medicineService.getMedicines(this.searchTerm()).subscribe({
      next: (data) => {
        this.medicines.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load medicines. Ensure the API is running on http://localhost:5151');
        this.loading.set(false);
      }
    });
  }

  onSearch(): void {
    this.loadMedicines();
  }

  clearSearch(): void {
    this.searchTerm.set('');
    this.loadMedicines();
  }

  toggleAddForm(): void {
    this.showAddForm.update((v) => !v);
  }

  onMedicineAdded(): void {
    this.showAddForm.set(false);
    this.loadMedicines();
  }

  getRowStatus(medicine: Medicine): RowStatus {
    const daysUntilExpiry = this.daysUntilExpiry(medicine.expiryDate);
    if (daysUntilExpiry < 30) {
      return 'expiring';
    }
    if (medicine.quantity < 10) {
      return 'low-stock';
    }
    return 'normal';
  }

  daysUntilExpiry(expiryDate: string): number {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const expiry = new Date(expiryDate + 'T00:00:00');
    const diffMs = expiry.getTime() - today.getTime();
    return Math.ceil(diffMs / (1000 * 60 * 60 * 24));
  }

  formatDate(date: string): string {
    return new Date(date + 'T00:00:00').toLocaleDateString();
  }

  formatPrice(price: number): string {
    return price.toFixed(2);
  }

  openSaleForm(medicineId: string): void {
    this.saleMedicineId.set(medicineId);
    this.saleQuantity.set(1);
    this.saleError.set(null);
    this.saleSuccess.set(null);
  }

  cancelSale(): void {
    this.saleMedicineId.set(null);
    this.saleError.set(null);
  }

  submitSale(medicine: Medicine): void {
    const qty = this.saleQuantity();
    if (qty <= 0) {
      this.saleError.set('Quantity must be greater than zero.');
      return;
    }
    if (qty > medicine.quantity) {
      this.saleError.set(`Only ${medicine.quantity} units available.`);
      return;
    }

    this.saleService.recordSale({ medicineId: medicine.id, quantitySold: qty }).subscribe({
      next: (sale) => {
        this.saleSuccess.set(
          `Sale recorded: ${sale.quantitySold} x ${sale.medicineName} ($${sale.totalPrice.toFixed(2)})`
        );
        this.saleMedicineId.set(null);
        this.loadMedicines();
      },
      error: (err) => {
        this.saleError.set(err.error?.message ?? 'Failed to record sale.');
      }
    });
  }
}
