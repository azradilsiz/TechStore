import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
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

  currentUserId = 2;

  constructor(
    private orderService: OrderService,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getOrders();
  }

  getOrders(): void {
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

  getPaymentLabel(hasPayment: boolean): string {
    return hasPayment ? 'Ödeme kaydı var' : 'Ödeme bekleniyor';
  }
}
