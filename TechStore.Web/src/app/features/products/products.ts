import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CartService } from '../../core/services/cart.service';
import { LocalCartService } from '../../core/services/local-cart.service';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../models/product.model';

type ProductPageStatus = 'loading' | 'success' | 'empty' | 'error';

@Component({
  selector: 'app-products',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class ProductsComponent implements OnInit {
  readonly maxQuantityPerProduct = 10;
  products: Product[] = [];
  status: ProductPageStatus = 'loading';
  errorMessage = '';
  successMessage = '';
  selectedCategory = '';
  searchTerm = '';

  readonly allCategoryLabel = 'Tüm Ürünler';
  constructor(
    private authService: AuthService,
    private productService: ProductService,
    private cartService: CartService,
    private localCartService: LocalCartService,
    private route: ActivatedRoute,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.searchTerm = params.get('search') ?? '';
      this.selectedCategory = params.get('category') ?? '';
      this.changeDetector.detectChanges();
    });

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

  get categories(): string[] {
    const categoryNames = this.products
      .map((product) => product.categoryName)
      .filter((categoryName) => categoryName);

    return Array.from(new Set(categoryNames));
  }

  get filteredProducts(): Product[] {
    const normalizedSearchTerm = this.searchTerm.trim().toLocaleLowerCase('tr-TR');
    const selectedCategoryKey = this.getCategoryKey(this.selectedCategory);

    return this.products.filter((product) => {
      const productCategoryKey = this.getCategoryKey(product.categoryName);
      const matchesCategory =
        !selectedCategoryKey ||
        productCategoryKey === selectedCategoryKey;
      const searchableText = `${product.name} ${product.description} ${this.getCategoryLabel(product.categoryName)}`.toLocaleLowerCase('tr-TR');
      const matchesSearch = !normalizedSearchTerm || searchableText.includes(normalizedSearchTerm);

      return matchesCategory && matchesSearch;
    });
  }

  selectCategory(categoryName: string): void {
    this.selectedCategory = categoryName;
    this.searchTerm = '';
  }

  onSearchTermChange(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.selectedCategory = '';
  }

  getCategoryLabel(categoryName: string): string {
    const categoryLabels: Record<string, string> = {
      'mobile-accessories': 'Aksesuar',
      smartphones: 'Telefon',
      laptops: 'Laptop',
      tablets: 'Tablet',
      mouse: 'Mouse'
    };

    return categoryLabels[this.getCategoryKey(categoryName)] ?? categoryName;
  }

  getStockLabel(stock: number): string {
    if (stock <= 0) {
      return 'Tükendi';
    }

    return stock <= 10 ? `Son ${stock} ürün` : '';
  }

  private getCategoryKey(categoryName: string): string {
    const normalizedCategoryName = categoryName.trim().toLocaleLowerCase('tr-TR');
    const categoryKeys: Record<string, string> = {
      telefon: 'smartphones',
      telephone: 'smartphones',
      smartphone: 'smartphones',
      smartphones: 'smartphones',
      laptop: 'laptops',
      laptops: 'laptops',
      tablet: 'tablets',
      tablets: 'tablets',
      mouse: 'mouse',
      aksesuar: 'mobile-accessories',
      accessory: 'mobile-accessories',
      accessories: 'mobile-accessories',
      'mobile-accessories': 'mobile-accessories'
    };

    return categoryKeys[normalizedCategoryName] ?? normalizedCategoryName;
  }

  addToCart(product: Product): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (product.stock <= 0) {
      this.errorMessage = 'Bu ürün stokta bulunmuyor.';
      this.changeDetector.detectChanges();
      return;
    }

    const currentUserId = this.authService.getCurrentUserId();

    if (!currentUserId) {
      const currentQuantity = this.localCartService.getCart().items
        .find((item) => item.productId === product.id)?.quantity ?? 0;
      if (currentQuantity >= Math.min(product.stock, this.maxQuantityPerProduct)) {
        this.errorMessage = 'Bu ürün için ekleyebileceğin en yüksek adede ulaştın.';
        this.changeDetector.detectChanges();
        return;
      }

      this.localCartService.addItem(product);
      this.successMessage = `${product.name} sepete eklendi.`;
      this.changeDetector.detectChanges();
      return;
    }

    this.cartService.addItemToCart({
      productId: product.id,
      quantity: 1
    }).subscribe({
      next: () => {
        this.successMessage = `${product.name} sepete eklendi.`;
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
