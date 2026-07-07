import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../core/services/order.service';
import { PaymentService } from '../../core/services/payment.service';
import { Order } from '../../models/order.model';
import { Payment } from '../../models/payment.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-checkout',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css'
})
export class CheckoutComponent {
  currentUserId = 2;
  userAddressId = 1;

  paymentMethod = 'Credit Card';

  isLoading = false;
  errorMessage = '';
  successMessage = '';

  createdOrder: Order | null = null;
  createdPayment: Payment | null = null;

  constructor(
    private orderService: OrderService,
    private paymentService: PaymentService,
    private changeDetector: ChangeDetectorRef
  ) {}

  completeCheckout(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.createdOrder = null;
    this.createdPayment = null;

    this.orderService.createOrderFromCart(this.currentUserId, {
      userAddressId: this.userAddressId
    }).subscribe({
      next: (order) => {
        this.createdOrder = order;
        this.createPayment(order.id);
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Sipariş oluşturulurken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  private createPayment(orderId: number): void {
    this.paymentService.createPayment(orderId, {
      paymentMethod: this.paymentMethod
    }).subscribe({
      next: (payment) => {
        this.createdPayment = payment;
        this.isLoading = false;
        this.successMessage = 'Siparişiniz ve ödemeniz başarıyla oluşturuldu.';
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Ödeme oluşturulurken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }
}