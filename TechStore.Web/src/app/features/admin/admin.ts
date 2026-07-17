import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-admin',
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class AdminComponent {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  get adminName(): string {
    const user = this.authService.getCurrentUser();
    return user?.fullName || user?.userName || 'Yönetici';
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
