import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, timeout } from 'rxjs';
import { CreatePayment, Payment, UpdatePayment } from '../../models/payment.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private readonly apiUrl = 'http://localhost:5050/api/Payments';

  constructor(private http: HttpClient) {}

  createPayment(orderId: number, dto: CreatePayment): Observable<Payment> {
    return this.http.post<Payment>(`${this.apiUrl}/order/${orderId}`, dto).pipe(
      timeout(10000)
    );
  }

  getPaymentById(paymentId: number): Observable<Payment> {
    return this.http.get<Payment>(`${this.apiUrl}/${paymentId}`).pipe(
      timeout(10000)
    );
  }

  updatePayment(paymentId: number, dto: UpdatePayment): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${paymentId}`, dto).pipe(
      timeout(10000)
    );
  }
}
