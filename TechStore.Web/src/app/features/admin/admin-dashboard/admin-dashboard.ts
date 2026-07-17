import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { OrderService } from '../../../core/services/order.service';
import { ProductService } from '../../../core/services/product.service';
import { UserService } from '../../../core/services/user.service';
import { Order } from '../../../models/order.model';

@Component({
  selector: 'app-admin-dashboard',
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboardComponent implements OnInit {
  productCount = 0;
  userCount = 0;
  pendingOrderCount = 0;
  lowStockCount = 0;
  recentOrders: Order[] = [];
  loading = true;

  constructor(private products: ProductService, private orders: OrderService, private users: UserService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    forkJoin({ products: this.products.getProducts(), orders: this.orders.getOrders(), users: this.users.getUsers() }).subscribe({
      next: data => {
        this.productCount = data.products.length;
        this.userCount = data.users.length;
        this.pendingOrderCount = data.orders.filter(order => ['Pending', 'Processing'].includes(order.status)).length;
        this.lowStockCount = data.products.filter(product => product.stock < 10).length;
        this.recentOrders = data.orders.sort((a,b) => +new Date(b.orderDate) - +new Date(a.orderDate)).slice(0, 5);
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => { this.loading = false; this.cdr.detectChanges(); }
    });
  }

  statusLabel(status: string): string {
    const labels: Record<string, string> = {
      Pending: 'Bekliyor',
      Processing: 'Hazırlanıyor',
      Shipped: 'Kargoda',
      Delivered: 'Teslim Edildi',
      Cancelled: 'İptal Edildi'
    };
    return labels[status] ?? status;
  }
}
