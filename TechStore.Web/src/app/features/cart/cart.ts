import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { Cart } from '../../models/cart.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cart',
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.html',
  styleUrl: './cart.css'
})
export class CartComponent implements OnInit {
  cart: Cart | null = null;
  isLoading = false;
  errorMessage = '';

  currentUserId = 2;

  constructor(
    private cartService: CartService,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getCart();
  }

  getCart(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.cartService.getCartByUserId(this.currentUserId).subscribe({
      next: (cart) => {
        this.cart = cart;
        this.isLoading = false;
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.cart = null;
        this.errorMessage = 'Sepet yüklenirken bir hata oluştu.';
        this.isLoading = false;
        this.changeDetector.detectChanges();
      }
    });
  }

  increaseQuantity(cartItemId: number, quantity: number): void {
    this.updateQuantity(cartItemId, quantity + 1);
  }

  decreaseQuantity(cartItemId: number, quantity: number): void {
    if (quantity <= 1) {
      return;
    }

    this.updateQuantity(cartItemId, quantity - 1);
  }

  updateQuantity(cartItemId: number, quantity: number): void {
    this.cartService.updateCartItem(cartItemId, { quantity }).subscribe({
      next: () => {
        this.getCart();
      },
      error: () => {
        this.errorMessage = 'Ürün adedi güncellenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  removeItem(cartItemId: number): void {
    this.cartService.removeCartItem(cartItemId).subscribe({
      next: () => {
        this.getCart();
      },
      error: () => {
        this.errorMessage = 'Ürün sepetten silinirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }
}
