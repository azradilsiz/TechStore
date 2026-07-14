import { Injectable } from '@angular/core';
import { Cart, CartItem } from '../../models/cart.model';
import { Product } from '../../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class LocalCartService {
  private readonly storageKey = 'techStoreGuestCart';

  getCart(): Cart {
    const items = this.getItems();

    return {
      id: 0,
      userId: 0,
      items,
      totalPrice: this.calculateTotalPrice(items)
    };
  }

  addItem(product: Product, quantity: number = 1): Cart {
    const items = this.getItems();
    const existingItem = items.find((item) => item.productId === product.id);

    if (existingItem) {
      existingItem.quantity += quantity;
      existingItem.totalPrice = this.roundPrice(existingItem.quantity * existingItem.unitPrice);
    } else {
      items.push({
        id: product.id,
        productId: product.id,
        productName: product.name,
        quantity,
        unitPrice: product.price,
        totalPrice: this.roundPrice(product.price * quantity)
      });
    }

    this.saveItems(items);

    return this.getCart();
  }

  updateItem(cartItemId: number, quantity: number): Cart {
    const items = this.getItems()
      .map((item) => item.id === cartItemId
        ? {
            ...item,
            quantity,
            totalPrice: this.roundPrice(item.unitPrice * quantity)
          }
        : item);

    this.saveItems(items);

    return this.getCart();
  }

  removeItem(cartItemId: number): Cart {
    const items = this.getItems().filter((item) => item.id !== cartItemId);

    this.saveItems(items);

    return this.getCart();
  }

  clearCart(): void {
    localStorage.removeItem(this.storageKey);
  }

  private getItems(): CartItem[] {
    const storedCart = localStorage.getItem(this.storageKey);

    if (!storedCart) {
      return [];
    }

    try {
      return JSON.parse(storedCart) as CartItem[];
    } catch {
      localStorage.removeItem(this.storageKey);
      return [];
    }
  }

  private saveItems(items: CartItem[]): void {
    localStorage.setItem(this.storageKey, JSON.stringify(items));
  }

  private calculateTotalPrice(items: CartItem[]): number {
    return this.roundPrice(items.reduce((total, item) => total + item.totalPrice, 0));
  }

  private roundPrice(price: number): number {
    return Math.round(price * 100) / 100;
  }
}
