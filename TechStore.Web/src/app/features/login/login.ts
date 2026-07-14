import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { LoginDto } from '../../models/auth.model';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  loginDto: LoginDto = {
    email: '',
    password: ''
  };

  isLoading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {}

  get returnUrl(): string {
    return this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
  }

  login(): void {
    this.errorMessage = '';

    if (!this.loginDto.email.trim() || !this.loginDto.password.trim()) {
      this.errorMessage = 'E-posta ve şifre alanlarını doldurmalısın.';
      return;
    }

    if (!this.isEmailValid(this.loginDto.email)) {
      this.errorMessage = 'Geçerli bir email adresi girmelisin.';
      return;
    }

    this.isLoading = true;

    this.authService.login(this.loginDto).subscribe({
      next: (user) => {
        this.isLoading = false;

        if (user.userTypeId === 1 || user.userTypeName.toLowerCase() === 'admin') {
          this.router.navigate(['/admin']);
          return;
        }

        this.router.navigateByUrl(this.returnUrl);
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'E-posta veya şifre hatalı.';
        this.changeDetector.detectChanges();
      }
    });
  }

  private isEmailValid(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
  }
}
