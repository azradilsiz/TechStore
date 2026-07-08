import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [FormsModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  searchTerm = '';

  constructor(private router: Router) {}

  searchProducts(): void {
    const query = this.searchTerm.trim();

    this.router.navigate(['/products'], {
      queryParams: query ? { search: query } : {}
    });
  }
}
