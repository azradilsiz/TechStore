import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CartService } from '../../core/services/cart.service';
import { LocalCartService } from '../../core/services/local-cart.service';
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
  readonly maxQuantityPerProduct = 10;
  product: Product | null = null;
  status: ProductDetailStatus = 'loading';
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService,
    private localCartService: LocalCartService,
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

  getStockLabel(stock: number): string {
    if (stock <= 0) {
      return 'Tükendi';
    }

    return stock <= 10 ? `Son ${stock} ürün` : '';
  }

  addToCart(): void {
    if (!this.product) {
      return;
    }

    this.successMessage = '';
    this.errorMessage = '';

    if (this.product.stock <= 0) {
      this.errorMessage = 'Bu ürün stokta bulunmuyor.';
      this.changeDetector.detectChanges();
      return;
    }

    const currentUserId = this.authService.getCurrentUserId();

    if (!currentUserId) {
      const currentQuantity = this.localCartService.getCart().items
        .find((item) => item.productId === this.product!.id)?.quantity ?? 0;
      if (currentQuantity >= Math.min(this.product.stock, this.maxQuantityPerProduct)) {
        this.errorMessage = 'Bu ürün için ekleyebileceğin en yüksek adede ulaştın.';
        this.changeDetector.detectChanges();
        return;
      }

      this.localCartService.addItem(this.product);
      this.successMessage = `${this.product.name} sepete eklendi.`;
      this.changeDetector.detectChanges();
      return;
    }

    this.cartService.addItemToCart({
      productId: this.product.id,
      quantity: 1
    }).subscribe({
      next: () => {
        this.successMessage = `${this.product?.name} sepete eklendi.`;
        this.changeDetector.detectChanges();
      },
      error: (error) => {
        this.errorMessage = error.status === 400
          ? 'Ürün stokta yok veya ürün başına en fazla 10 adet ekleyebilirsin.'
          : 'Ürün sepete eklenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }
}
