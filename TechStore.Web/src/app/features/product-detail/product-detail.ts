import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../models/product.model';

type ProductDetailStatus = 'loading' | 'success' | 'error';

@Component({
  selector: 'app-product-detail',
  imports: [CommonModule, RouterLink],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.css'
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  status: ProductDetailStatus = 'loading';
  errorMessage = '';
  successMessage = '';

  currentUserId = 2;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const productId = Number(this.route.snapshot.paramMap.get('productId'));

    if (!productId) {
      this.status = 'error';
      this.errorMessage = 'Ürün bilgisi bulunamadı.';
      return;
    }

    this.getProduct(productId);
  }

  getProduct(productId: number): void {
    this.status = 'loading';
    this.errorMessage = '';

    this.productService.getProductById(productId).subscribe({
      next: (product) => {
        this.product = product;
        this.status = 'success';
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.product = null;
        this.status = 'error';
        this.errorMessage = 'Ürün detayı yüklenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  getCategoryLabel(categoryName: string): string {
    const categoryLabels: Record<string, string> = {
      'mobile-accessories': 'Aksesuar',
      smartphones: 'Telefon',
      laptops: 'Laptop',
      tablets: 'Tablet',
      telephone: 'Telefon'
    };

    return categoryLabels[categoryName] ?? categoryName;
  }

  addToCart(): void {
    if (!this.product) {
      return;
    }

    this.successMessage = '';
    this.errorMessage = '';

    this.cartService.addItemToCart({
      userId: this.currentUserId,
      productId: this.product.id,
      quantity: 1
    }).subscribe({
      next: () => {
        this.successMessage = `${this.product?.name} sepete eklendi.`;
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Ürün sepete eklenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }
}
