import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: '../login/login.css'
})
export class ForgotPasswordComponent {
  email = '';
  message = '';
  errorMessage = '';

  sendResetRequest(): void {
    this.message = '';
    this.errorMessage = '';

    if (!this.email.trim()) {
      this.errorMessage = 'E-posta alanını doldurmalısın.';
      return;
    }

    if (!this.isEmailValid(this.email)) {
      this.errorMessage = 'Geçerli bir e-posta adresi girmelisin.';
      return;
    }

    this.message = 'Bu e-posta adresiyle kayıtlı bir hesap varsa, e-posta gönderme altyapısı bağlandığında şifre yenileme bağlantısı gönderilecek.';
  }

  private isEmailValid(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
  }
}
