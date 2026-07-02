import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, timeout } from 'rxjs';
import { AddCartItem, Cart, UpdateCartItem } from '../../models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly apiUrl = 'http://localhost:5050/api/Carts';

  constructor(private http: HttpClient) {}

  getCartByUserId(userId: number): Observable<Cart> {
    return this.http.get<Cart>(`${this.apiUrl}/user/${userId}`).pipe(
      timeout(10000)
    );
  }

  addItemToCart(dto: AddCartItem): Observable<Cart> {
    return this.http.post<Cart>(`${this.apiUrl}/add-item`, dto).pipe(
      timeout(10000)
    );
  }

  updateCartItem(cartItemId: number, dto: UpdateCartItem): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/items/${cartItemId}`, dto).pipe(
      timeout(10000)
    );
  }

  removeCartItem(cartItemId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/items/${cartItemId}`).pipe(
      timeout(10000)
    );
  }
}