import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, timeout } from 'rxjs';
import { CreateGuestOrder, CreateOrderFromCart, Order } from '../../models/order.model';

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

  createGuestOrder(dto: CreateGuestOrder): Observable<Order> {
    return this.http.post<Order>(`${this.apiUrl}/guest`, dto).pipe(
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

  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(this.apiUrl).pipe(timeout(10000));
  }

  updateOrderStatus(orderId: number, status: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${orderId}`, { status }).pipe(timeout(10000));
  }
}
