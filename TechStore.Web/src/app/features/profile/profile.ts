import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { UserAddressService } from '../../core/services/user-address.service';
import { AuthResponseDto, ChangePasswordDto } from '../../models/auth.model';
import { CreateUserAddress, UpdateUserAddress, UserAddress } from '../../models/user-address.model';

@Component({
  selector: 'app-profile',
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class ProfileComponent implements OnInit {
  currentUser: AuthResponseDto | null = null;
  addresses: UserAddress[] = [];
  newAddress: CreateUserAddress = this.createEmptyAddress();
  editingAddressId: number | null = null;
  passwordForm = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  };

  isAddressFormVisible = false;
  isAddressLoading = false;
  isAddressSaving = false;
  isPasswordSaving = false;
  errorMessage = '';
  successMessage = '';
  passwordErrorMessage = '';
  passwordSuccessMessage = '';

  constructor(
    private authService: AuthService,
    private userAddressService: UserAddressService,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();

    if (!this.currentUser) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: '/profile' }
      });
      return;
    }

    this.newAddress = this.createEmptyAddress();
    this.getUserAddresses();
  }

  get fullName(): string {
    return this.currentUser?.fullName || this.currentUser?.userName || '';
  }

  get addressFormTitle(): string {
    return this.editingAddressId === null ? 'Yeni adres ekle' : 'Adresi düzenle';
  }

  get addressSaveButtonText(): string {
    if (this.isAddressSaving) {
      return this.editingAddressId === null ? 'Adres kaydediliyor...' : 'Adres güncelleniyor...';
    }

    return this.editingAddressId === null ? 'Adresi Kaydet' : 'Adresi Güncelle';
  }

  toggleAddressForm(): void {
    if (this.isAddressFormVisible) {
      this.cancelAddressForm();
      return;
    }

    this.isAddressFormVisible = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  editAddress(address: UserAddress): void {
    if (!this.currentUser) {
      return;
    }

    this.editingAddressId = address.id;
    this.isAddressFormVisible = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.newAddress = {
      userId: this.currentUser.userId,
      title: address.title,
      city: address.city,
      district: address.district,
      addressDetail: address.addressDetail,
      phone: address.phone
    };
  }

  cancelAddressForm(): void {
    this.isAddressFormVisible = false;
    this.editingAddressId = null;
    this.errorMessage = '';
    this.newAddress = this.createEmptyAddress();
  }

  saveAddress(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.currentUser) {
      return;
    }

    if (!this.isAddressFormValid()) {
      this.errorMessage = 'Adres başlığı, şehir, ilçe, adres detayı ve geçerli telefon alanlarını doldurmalısın.';
      return;
    }

    this.isAddressSaving = true;

    if (this.editingAddressId !== null) {
      this.updateAddress(this.editingAddressId);
      return;
    }

    this.userAddressService.createUserAddress({
      ...this.newAddress,
      userId: this.currentUser.userId,
      title: this.newAddress.title.trim(),
      city: this.newAddress.city.trim(),
      district: this.newAddress.district.trim(),
      addressDetail: this.newAddress.addressDetail.trim(),
      phone: this.normalizePhone(this.newAddress.phone)
    }).subscribe({
      next: (address) => {
        this.addresses = [...this.addresses, address];
        this.cancelAddressForm();
        this.isAddressSaving = false;
        this.successMessage = 'Adres başarıyla eklendi.';
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.isAddressSaving = false;
        this.errorMessage = 'Adres eklenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  deleteAddress(addressId: number): void {
    this.errorMessage = '';
    this.successMessage = '';

    this.userAddressService.deleteUserAddress(addressId).subscribe({
      next: () => {
        this.addresses = this.addresses.filter((address) => address.id !== addressId);

        if (this.editingAddressId === addressId) {
          this.cancelAddressForm();
        }

        this.successMessage = 'Adres başarıyla silindi.';
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Adres silinirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  changePassword(): void {
    this.passwordErrorMessage = '';
    this.passwordSuccessMessage = '';

    if (!this.currentUser) {
      return;
    }

    if (!this.passwordForm.currentPassword.trim() || !this.passwordForm.newPassword.trim() || !this.passwordForm.confirmPassword.trim()) {
      this.passwordErrorMessage = 'Mevcut şifre, yeni şifre ve şifre tekrarı alanlarını doldurmalısın.';
      return;
    }

    if (this.passwordForm.newPassword.trim().length < 6) {
      this.passwordErrorMessage = 'Yeni şifre en az 6 karakter olmalı.';
      return;
    }

    if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
      this.passwordErrorMessage = 'Yeni şifre ve şifre tekrarı aynı olmalı.';
      return;
    }

    const dto: ChangePasswordDto = {
      userId: this.currentUser.userId,
      currentPassword: this.passwordForm.currentPassword,
      newPassword: this.passwordForm.newPassword
    };

    this.isPasswordSaving = true;

    this.authService.changePassword(dto).subscribe({
      next: () => {
        this.passwordForm = {
          currentPassword: '',
          newPassword: '',
          confirmPassword: ''
        };
        this.isPasswordSaving = false;
        this.passwordSuccessMessage = 'Şifren başarıyla güncellendi.';
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.isPasswordSaving = false;
        this.passwordErrorMessage = 'Mevcut şifre hatalı veya şifre güncellenemedi.';
        this.changeDetector.detectChanges();
      }
    });
  }

  private updateAddress(addressId: number): void {
    const dto: UpdateUserAddress = {
      title: this.newAddress.title.trim(),
      city: this.newAddress.city.trim(),
      district: this.newAddress.district.trim(),
      addressDetail: this.newAddress.addressDetail.trim(),
      phone: this.normalizePhone(this.newAddress.phone)
    };

    this.userAddressService.updateUserAddress(addressId, dto).subscribe({
      next: () => {
        this.addresses = this.addresses.map((address) => (
          address.id === addressId ? { ...address, ...dto } : address
        ));
        this.cancelAddressForm();
        this.isAddressSaving = false;
        this.successMessage = 'Adres başarıyla güncellendi.';
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.isAddressSaving = false;
        this.errorMessage = 'Adres güncellenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  private getUserAddresses(): void {
    if (!this.currentUser) {
      return;
    }

    this.isAddressLoading = true;

    this.userAddressService.getUserAddressesByUserId(this.currentUser.userId).subscribe({
      next: (addresses) => {
        this.addresses = addresses;
        this.isAddressLoading = false;
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.addresses = [];
        this.isAddressLoading = false;
        this.errorMessage = 'Adresler yüklenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  private createEmptyAddress(): CreateUserAddress {
    return {
      userId: this.currentUser?.userId ?? 0,
      title: '',
      city: '',
      district: '',
      addressDetail: '',
      phone: ''
    };
  }

  private isAddressFormValid(): boolean {
    return Boolean(
      this.newAddress.title.trim() &&
      this.newAddress.city.trim() &&
      this.newAddress.district.trim() &&
      this.newAddress.addressDetail.trim() &&
      this.isPhoneValid(this.newAddress.phone)
    );
  }

  private isPhoneValid(phone: string): boolean {
    return /^(?:5\d{9}|05\d{9})$/.test(this.normalizePhone(phone));
  }

  private normalizePhone(phone: string): string {
    return phone.replace(/\s+/g, '');
  }
}
