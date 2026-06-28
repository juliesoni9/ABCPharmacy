import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateMedicineRequest, Medicine } from '../models/medicine.model';
import { environment } from '../../environments/environment';

const API_BASE = environment.apiUrl;

@Injectable({ providedIn: 'root' })
export class MedicineService {
  constructor(private http: HttpClient) {}

  getMedicines(search?: string): Observable<Medicine[]> {
    let params = new HttpParams();
    if (search?.trim()) {
      params = params.set('search', search.trim());
    }
    return this.http.get<Medicine[]>(`${API_BASE}/medicines`, { params });
  }

  createMedicine(request: CreateMedicineRequest): Observable<Medicine> {
    return this.http.post<Medicine>(`${API_BASE}/medicines`, request);
  }
}
