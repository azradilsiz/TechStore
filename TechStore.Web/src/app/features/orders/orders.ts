import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { OrderService } from '../../core/services/order.service';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-orders',
  imports: [CommonModule],
  templateUrl: './orders.html',
  styleUrl: './orders.css'
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];

  isLoading = false;
  errorMessage = '';
  currentUserId: number | null = null;

  constructor(
    private authService: AuthService,
    private orderService: OrderService,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.authService.getCurrentUserId();

    if (!this.currentUserId) {
      this.router.navigate(['/login']);
      return;
    }

    this.getOrders();
  }

  getOrders(): void {
    if (!this.currentUserId) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.orderService.getOrdersByUserId(this.currentUserId).subscribe({
      next: (orders) => {
        this.orders = orders;
        this.isLoading = false;
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.orders = [];
        this.errorMessage = 'Siparişler yüklenirken bir hata oluştu.';
        this.isLoading = false;
        this.changeDetector.detectChanges();
      }
    });
  }

  getOrderStatusLabel(status: string): string {
    const statusLabels: Record<string, string> = {
      pending: 'Sipariş alındı',
      processing: 'Hazırlanıyor',
      shipped: 'Kargoda',
      delivered: 'Teslim edildi',
      completed: 'Tamamlandı',
      cancelled: 'İptal edildi'
    };

    return statusLabels[status.trim().toLocaleLowerCase('tr-TR')] ?? status;
  }

  isOrderCancelled(status: string): boolean {
    return status.trim().toLocaleLowerCase('tr-TR') === 'cancelled';
  }

  getPaymentMethodLabel(paymentMethod: string | null | undefined): string {
    const labels: Record<string, string> = {
      'Credit Card': 'Kredi Kartı',
      'Bank Transfer': 'Banka Havalesi',
      Cash: 'Kapıda Ödeme'
    };

    return paymentMethod ? (labels[paymentMethod] ?? paymentMethod) : 'Belirtilmemiş';
  }

  getPaymentStatusLabel(paymentStatus: string | null | undefined): string {
    const labels: Record<string, string> = {
      Pending: 'Ödeme bekleniyor',
      Paid: 'Ödendi',
      PayOnDelivery: 'Teslimatta ödenecek',
      Failed: 'Ödeme başarısız',
      Refunded: 'İade edildi',
      Cancelled: 'Ödeme iptal edildi'
    };

    return paymentStatus
      ? (labels[paymentStatus] ?? paymentStatus)
      : 'Ödeme Bilgisi Bulunamadı';
  }

  getPaymentStatusClass(paymentStatus: string | null | undefined): string {
    return paymentStatus?.trim().toLocaleLowerCase('tr-TR') || 'unknown';
  }
}
