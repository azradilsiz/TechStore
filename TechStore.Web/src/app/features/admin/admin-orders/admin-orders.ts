import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../../core/services/order.service';
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

  constructor(private orderService: OrderService, private cdr: ChangeDetectorRef) {}
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
    this.orderService.updateOrderStatus(order.id, status).subscribe({ error: () => { order.status = previous; this.errorMessage = 'Sipariş durumu güncellenemedi.'; this.cdr.detectChanges(); } });
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
