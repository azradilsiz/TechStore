import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../../core/services/order.service';
import { PaymentService } from '../../../core/services/payment.service';
import { Order } from '../../../models/order.model';

@Component({ selector: 'app-admin-orders', imports: [CommonModule, FormsModule], templateUrl: './admin-orders.html', styleUrl: './admin-orders.css' })
export class AdminOrdersComponent implements OnInit {
  orders: Order[] = [];
  loading = true;
  errorMessage = '';
  statusFilter = '';
  readonly statuses = [
    { value: 'Pending', label: 'Bekliyor' },
    { value: 'Processing', label: 'Hazırlanıyor' },
    { value: 'Shipped', label: 'Kargoda' },
    { value: 'Delivered', label: 'Teslim Edildi' },
    { value: 'Cancelled', label: 'İptal Edildi' }
  ];
  readonly paymentStatuses = [
    { value: 'Pending', label: 'Ödeme Bekleniyor' },
    { value: 'Paid', label: 'Ödendi' },
    { value: 'PayOnDelivery', label: 'Teslimatta Ödenecek' },
    { value: 'Failed', label: 'Ödeme Başarısız' },
    { value: 'Refunded', label: 'İade Edildi' },
    { value: 'Cancelled', label: 'Ödeme İptal Edildi' }
  ];

  constructor(
    private orderService: OrderService,
    private paymentService: PaymentService,
    private cdr: ChangeDetectorRef
  ) {}
  ngOnInit(): void { this.loadOrders(); }
  get filteredOrders(): Order[] { return this.statusFilter ? this.orders.filter(o => o.status === this.statusFilter) : this.orders; }

  loadOrders(): void {
    this.loading = true;
    this.orderService.getOrders().subscribe({
      next: orders => { this.orders = orders.map(order => ({ ...order, status: this.normalizeStatus(order.status) })).sort((a,b) => +new Date(b.orderDate) - +new Date(a.orderDate)); this.loading = false; this.cdr.detectChanges(); },
      error: () => { this.errorMessage = 'Siparişler yüklenemedi.'; this.loading = false; this.cdr.detectChanges(); }
    });
  }

  changeStatus(order: Order, status: string): void {
    const previous = order.status;
    order.status = status;
    this.orderService.updateOrderStatus(order.id, status).subscribe({
      next: () => this.loadOrders(),
      error: () => {
        order.status = previous;
        this.errorMessage = 'Sipariş durumu güncellenemedi.';
        this.cdr.detectChanges();
      }
    });
  }

  changePaymentStatus(order: Order, paymentStatus: string): void {
    if (!order.paymentId) {
      this.errorMessage = 'Bu siparişe ait ödeme kaydı bulunamadı.';
      return;
    }

    const previous = order.paymentStatus;
    order.paymentStatus = paymentStatus;
    this.paymentService.updatePayment(order.paymentId, { paymentStatus }).subscribe({
      next: () => this.loadOrders(),
      error: () => {
        order.paymentStatus = previous;
        this.errorMessage = 'Ödeme durumu güncellenemedi.';
        this.cdr.detectChanges();
      }
    });
  }

  paymentMethodLabel(paymentMethod: string): string {
    const labels: Record<string, string> = {
      'Credit Card': 'Kredi Kartı',
      'Bank Transfer': 'Banka Havalesi',
      Cash: 'Kapıda Ödeme'
    };
    return labels[paymentMethod] ?? '-';
  }

  customerName(order: Order): string { return order.userName || order.guestFullName || 'Misafir müşteri'; }

  totalQuantity(order: Order): number {
    return order.items.reduce((total, item) => total + item.quantity, 0);
  }

  private normalizeStatus(status: string): string {
    const legacyStatuses: Record<string, string> = {
      Bekliyor: 'Pending', Hazırlanıyor: 'Processing', Kargoda: 'Shipped',
      'Teslim Edildi': 'Delivered', 'İptal Edildi': 'Cancelled'
    };
    return legacyStatuses[status] ?? status;
  }
}
