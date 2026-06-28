import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SaleService } from '../../services/sale.service';
import { SaleRecord } from '../../models/sale.model';

@Component({
  selector: 'app-sales-list',
  imports: [CommonModule],
  templateUrl: './sales-list.html',
  styleUrl: './sales-list.scss'
})
export class SalesListComponent implements OnInit {
  sales = signal<SaleRecord[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  constructor(private saleService: SaleService) {}

  ngOnInit(): void {
    this.loadSales();
  }

  loadSales(): void {
    this.loading.set(true);
    this.saleService.getSales().subscribe({
      next: (data) => {
        this.sales.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load sale records.');
        this.loading.set(false);
      }
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleString();
  }

  formatPrice(price: number): string {
    return price.toFixed(2);
  }
}
