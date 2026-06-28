import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateSaleRequest, SaleRecord } from '../models/sale.model';
import { environment } from '../../environments/environment';

const API_BASE = environment.apiUrl;

@Injectable({ providedIn: 'root' })
export class SaleService {
  constructor(private http: HttpClient) {}

  getSales(): Observable<SaleRecord[]> {
    return this.http.get<SaleRecord[]>(`${API_BASE}/sales`);
  }

  recordSale(request: CreateSaleRequest): Observable<SaleRecord> {
    return this.http.post<SaleRecord>(`${API_BASE}/sales`, request);
  }
}
