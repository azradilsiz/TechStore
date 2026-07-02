import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../models/product.model';

type ProductPageStatus = 'loading' | 'success' | 'empty' | 'error';

@Component({
  selector: 'app-products',
  imports: [CommonModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: Product[] = [];
  status: ProductPageStatus = 'loading';
  errorMessage = '';
  successMessage = '';

  currentUserId = 2;

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getProducts();
  }

  getProducts(): void {
    this.status = 'loading';
    this.errorMessage = '';
    this.successMessage = '';

    this.productService.getProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.status = products.length > 0 ? 'success' : 'empty';
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.products = [];
        this.status = 'error';
        this.errorMessage = 'Ürünler yüklenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  addToCart(product: Product): void {
    this.errorMessage = '';
    this.successMessage = '';

    this.cartService.addItemToCart({
      userId: this.currentUserId,
      productId: product.id,
      quantity: 1
    }).subscribe({
      next: () => {
        this.successMessage = `${product.name} sepete eklendi.`;
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Ürün sepete eklenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }
}
