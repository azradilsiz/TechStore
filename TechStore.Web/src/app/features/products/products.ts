import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../models/product.model';

type ProductPageStatus = 'loading' | 'success' | 'empty' | 'error';

@Component({
  selector: 'app-products',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: Product[] = [];
  status: ProductPageStatus = 'loading';
  errorMessage = '';
  successMessage = '';
  selectedCategory = '';
  searchTerm = '';

  readonly allCategoryLabel = 'Tüm Ürünler';
  currentUserId = 2;

  constructor(
    private productService: ProductService,
    private cartService: CartService,
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

    return this.products.filter((product) => {
      const matchesCategory = !this.selectedCategory || product.categoryName === this.selectedCategory;
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
      telephone: 'Telefon'
    };

    return categoryLabels[categoryName] ?? categoryName;
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
