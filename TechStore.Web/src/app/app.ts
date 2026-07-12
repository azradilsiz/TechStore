import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from './core/services/auth.service';
import { AuthResponseDto } from './models/auth.model';

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
  currentUser: AuthResponseDto | null = null;
  isAuthPage = false;

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

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
    this.authService.currentUser$.subscribe((user) => {
      this.currentUser = user;
    });

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event) => {
        this.isAuthPage = event.urlAfterRedirects.startsWith('/login') ||
          event.urlAfterRedirects.startsWith('/register');
      });
  }

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

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  getUserInitials(): string {
    if (!this.currentUser) {
      return '';
    }

    const displayName = this.currentUser.fullName || this.currentUser.userName;

    return displayName
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((namePart) => namePart.charAt(0).toUpperCase())
      .join('');
  }
}
