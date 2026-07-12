import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { RegisterDto } from '../../models/auth.model';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {
  registerDto: RegisterDto = {
    userName: '',
    password: '',
    firstName: '',
    lastName: '',
    email: ''
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

  register(): void {
    this.errorMessage = '';

    if (!this.isFormValid()) {
      this.errorMessage = 'Tüm alanları doldurmalısın.';
      return;
    }

    this.isLoading = true;

    this.authService.register(this.registerDto).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigateByUrl(this.returnUrl);
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Kayıt oluşturulurken bir hata oluştu. Bu email daha önce kullanılmış olabilir.';
        this.changeDetector.detectChanges();
      }
    });
  }

  private isFormValid(): boolean {
    return Boolean(
      this.registerDto.userName.trim() &&
      this.registerDto.password.trim() &&
      this.registerDto.firstName.trim() &&
      this.registerDto.lastName.trim() &&
      this.registerDto.email.trim()
    );
  }
}
