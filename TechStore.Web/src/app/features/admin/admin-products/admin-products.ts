import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin, Observable } from 'rxjs';
import { CategoryService } from '../../../core/services/category.service';
import { ProductService } from '../../../core/services/product.service';
import { Category } from '../../../models/category.model';
import { Product, ProductWriteDto } from '../../../models/product.model';

@Component({
  selector: 'app-admin-products',
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-products.html',
  styleUrl: './admin-products.css'
})
export class AdminProductsComponent implements OnInit {
  products: Product[] = [];
  categories: Category[] = [];
  searchTerm = '';
  loading = true;
  saving = false;
  errorMessage = '';
  successMessage = '';
  editingId: number | null = null;
  form: ProductWriteDto = this.emptyForm();

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void { this.loadData(); }

  get filteredProducts(): Product[] {
    const search = this.searchTerm.trim().toLocaleLowerCase('tr-TR');
    return !search ? this.products : this.products.filter((product) =>
      `${product.name} ${product.categoryName}`.toLocaleLowerCase('tr-TR').includes(search));
  }

  stockStatus(product: Product): 'out' | 'low' | 'normal' {
    if (product.stock === 0) return 'out';
    if (product.stock < 10) return 'low';
    return 'normal';
  }

  loadData(): void {
    this.loading = true;
    forkJoin({ products: this.productService.getProducts(), categories: this.categoryService.getCategories() })
      .subscribe({
        next: ({ products, categories }) => {
          this.products = products;
          this.categories = categories;
          this.loading = false;
          this.cdr.detectChanges();
        },
        error: () => this.showError('Ürünler yüklenemedi.')
      });
  }

  edit(product: Product): void {
    this.editingId = product.id;
    this.form = { categoryId: product.categoryId, name: product.name, description: product.description,
      price: product.price, stock: product.stock, imageUrl: product.imageUrl };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.form = this.emptyForm();
  }

  save(): void {
    if (!this.form.name.trim() || this.form.categoryId <= 0 || this.form.price <= 0 || this.form.stock < 0) {
      this.showError('Ürün adı, kategori, fiyat ve stok bilgilerini kontrol edin.');
      return;
    }
    this.saving = true;
    this.errorMessage = '';
    const request: Observable<unknown> = this.editingId
      ? this.productService.updateProduct(this.editingId, this.form)
      : this.productService.createProduct(this.form);
    request.subscribe({
      next: () => {
        this.successMessage = this.editingId ? 'Ürün güncellendi.' : 'Ürün eklendi.';
        this.cancelEdit();
        this.saving = false;
        this.loadData();
      },
      error: () => { this.saving = false; this.showError('Ürün kaydedilemedi.'); }
    });
  }

  remove(product: Product): void {
    if (!confirm(`“${product.name}” ürününü silmek istediğinize emin misiniz?`)) return;
    this.productService.deleteProduct(product.id).subscribe({
      next: () => { this.successMessage = 'Ürün silindi.'; this.loadData(); },
      error: () => this.showError('Ürün silinemedi.')
    });
  }

  private emptyForm(): ProductWriteDto {
    return { categoryId: 0, name: '', description: '', price: 0, stock: 0, imageUrl: '' };
  }

  private showError(message: string): void {
    this.errorMessage = message;
    this.loading = false;
    this.cdr.detectChanges();
  }
}
