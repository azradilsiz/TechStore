import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, timeout } from 'rxjs';
import { Product, ProductWriteDto } from '../../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly apiUrl = 'http://localhost:5050/api/Products';

  constructor(private http: HttpClient) {}

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl).pipe(
      timeout(10000)
    );
  }

  getProductById(productId: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${productId}`).pipe(
      timeout(10000)
    );
  }

  createProduct(dto: ProductWriteDto): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, dto).pipe(timeout(10000));
  }

  updateProduct(productId: number, dto: ProductWriteDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${productId}`, dto).pipe(timeout(10000));
  }

  deleteProduct(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${productId}`).pipe(timeout(10000));
  }
}
