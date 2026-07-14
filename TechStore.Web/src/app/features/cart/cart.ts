import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CartService } from '../../core/services/cart.service';
import { LocalCartService } from '../../core/services/local-cart.service';
import { Cart } from '../../models/cart.model';

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
  currentUserId: number | null = null;

  constructor(
    private authService: AuthService,
    private cartService: CartService,
    private localCartService: LocalCartService,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.authService.getCurrentUserId();
    this.getCart();
  }

  getCart(): void {
    this.errorMessage = '';

    if (!this.currentUserId) {
      this.cart = this.localCartService.getCart();
      return;
    }

    this.isLoading = true;

    this.cartService.getCartByUserId(this.currentUserId).subscribe({
      next: (cart) => {
        this.cart = cart;
        this.isLoading = false;
        this.changeDetector.detectChanges();
      },
      error: (error: HttpErrorResponse) => {
        this.cart = null;
        this.errorMessage = error.status === 404 ? '' : 'Sepet yüklenirken bir hata oluştu.';
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
    if (!this.currentUserId) {
      this.cart = this.localCartService.updateItem(cartItemId, quantity);
      return;
    }

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
    if (!this.currentUserId) {
      this.cart = this.localCartService.removeItem(cartItemId);
      return;
    }

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

  formatPrice(price: number): string {
    return price.toFixed(2);
  }
}
