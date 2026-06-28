import { Component, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MedicineService } from '../../services/medicine.service';
import { CreateMedicineRequest } from '../../models/medicine.model';

@Component({
  selector: 'app-medicine-form',
  imports: [CommonModule, FormsModule],
  templateUrl: './medicine-form.html',
  styleUrl: './medicine-form.scss'
})
export class MedicineFormComponent {
  saved = output<void>();
  cancelled = output<void>();

  form: CreateMedicineRequest = {
    fullName: '',
    notes: '',
    expiryDate: '',
    quantity: 0,
    price: 0,
    brand: ''
  };

  submitting = false;
  error: string | null = null;

  constructor(private medicineService: MedicineService) {}

  onSubmit(): void {
    this.error = null;

    if (!this.form.fullName.trim() || !this.form.brand.trim() || !this.form.expiryDate) {
      this.error = 'Full Name, Brand, and Expiry Date are required.';
      return;
    }

    if (this.form.quantity < 0 || this.form.price < 0) {
      this.error = 'Quantity and Price must be zero or greater.';
      return;
    }

    this.submitting = true;
    this.medicineService
      .createMedicine({
        ...this.form,
        fullName: this.form.fullName.trim(),
        notes: this.form.notes.trim(),
        brand: this.form.brand.trim(),
        price: Math.round(this.form.price * 100) / 100
      })
      .subscribe({
        next: () => {
          this.submitting = false;
          this.saved.emit();
        },
        error: (err) => {
          this.submitting = false;
          this.error = err.error?.message ?? err.error?.title ?? 'Failed to add medicine.';
        }
      });
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
