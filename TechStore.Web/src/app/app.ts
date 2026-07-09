import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

interface MenuCategory {
  label: string;
  value: string;
}

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  searchTerm = '';
  isCategoryMenuOpen = false;

  readonly menuCategories: MenuCategory[] = [
    { label: 'Telefon', value: 'smartphones' },
    { label: 'Laptop', value: 'laptops' },
    { label: 'Tablet', value: 'tablets' },
    { label: 'Mouse', value: 'mouse' },
    { label: 'Aksesuar', value: 'mobile-accessories' }
  ];

  readonly popularSearches = [
    'iPhone',
    'Samsung',
    'MacBook',
    'Logitech'
  ];

  constructor(private router: Router) {}

  searchProducts(): void {
    const query = this.searchTerm.trim();

    this.router.navigate(['/products'], {
      queryParams: query ? { search: query } : {}
    });
  }

  toggleCategoryMenu(): void {
    this.isCategoryMenuOpen = !this.isCategoryMenuOpen;
  }

  closeCategoryMenu(): void {
    this.isCategoryMenuOpen = false;
  }
}
