import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, timeout } from 'rxjs';
import { CreateOrderFromCart, Order } from '../../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly apiUrl = 'http://localhost:5050/api/Orders';

  constructor(private http: HttpClient) {}

  createOrderFromCart(userId: number, dto: CreateOrderFromCart): Observable<Order> {
    return this.http.post<Order>(`${this.apiUrl}/user/${userId}/from-cart`, dto).pipe(
      timeout(10000)
    );
  }

  getOrderById(orderId: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/${orderId}`).pipe(
      timeout(10000)
    );
  }

  getOrdersByUserId(userId: number): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/user/${userId}`).pipe(
      timeout(10000)
    );
  }
}