import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { LocalCartService } from '../../core/services/local-cart.service';
import { OrderService } from '../../core/services/order.service';
import { UserAddressService } from '../../core/services/user-address.service';
import { Cart } from '../../models/cart.model';
import { Order } from '../../models/order.model';
import { CreateUserAddress, UserAddress } from '../../models/user-address.model';

type CheckoutMode = 'account' | 'guest';

interface GuestCheckoutForm {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  city: string;
  district: string;
  addressDetail: string;
}

@Component({
  selector: 'app-checkout',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css'
})
export class CheckoutComponent implements OnInit {
  currentUserId: number | null = null;
  checkoutMode: CheckoutMode | null = null;

  selectedAddressId: number | null = null;
  addresses: UserAddress[] = [];
  newAddress: CreateUserAddress = this.createEmptyAddress();
  isAddressFormVisible = false;

  guestCart: Cart = {
    id: 0,
    userId: 0,
    items: [],
    totalPrice: 0
  };
  guestForm: GuestCheckoutForm = this.createEmptyGuestForm();

  paymentMethod = 'Credit Card';

  isLoading = false;
  isAddressLoading = false;
  isAddressSaving = false;
  errorMessage = '';
  addressErrorMessage = '';
  addressSuccessMessage = '';
  successMessage = '';

  createdOrder: Order | null = null;

  constructor(
    private authService: AuthService,
    private orderService: OrderService,
    private userAddressService: UserAddressService,
    private localCartService: LocalCartService,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.authService.getCurrentUserId();
    this.checkoutMode = this.currentUserId ? 'account' : null;

    if (this.currentUserId) {
      this.newAddress = this.createEmptyAddress();
      this.getUserAddresses();
    } else {
      this.guestCart = this.localCartService.getCart();
    }
  }

  get selectedAddress(): UserAddress | undefined {
    return this.addresses.find((address) => address.id === this.selectedAddressId);
  }

  get isGuestMode(): boolean {
    return this.checkoutMode === 'guest';
  }

  get isAccountMode(): boolean {
    return this.checkoutMode === 'account';
  }

  get canShowCheckoutForm(): boolean {
    return this.isAccountMode || this.isGuestMode;
  }

  get isOrderCreated(): boolean {
    return this.createdOrder !== null;
  }

  get guestItemCount(): number {
    return this.guestCart.items.reduce((total, item) => total + item.quantity, 0);
  }

  getPaymentStatusLabel(paymentStatus: string): string {
    const labels: Record<string, string> = {
      Pending: 'Ödeme bekleniyor',
      Paid: 'Ödendi',
      PayOnDelivery: 'Teslimatta ödenecek',
      Failed: 'Ödeme başarısız',
      Refunded: 'İade edildi',
      Cancelled: 'Ödeme iptal edildi'
    };

    return labels[paymentStatus] ?? 'Ödeme bilgisi bulunamadı';
  }

  continueAsGuest(): void {
    this.checkoutMode = 'guest';
    this.errorMessage = '';
    this.successMessage = '';
    this.guestCart = this.localCartService.getCart();
  }

  goToLogin(): void {
    this.router.navigate(['/login'], {
      queryParams: { returnUrl: '/checkout' }
    });
  }

  goToProducts(): void {
    this.router.navigate(['/products']);
  }

  getUserAddresses(): void {
    if (!this.currentUserId) {
      return;
    }

    this.isAddressLoading = true;
    this.addressErrorMessage = '';

    this.userAddressService.getUserAddressesByUserId(this.currentUserId).subscribe({
      next: (addresses) => {
        this.addresses = addresses;
        this.selectedAddressId = addresses.length > 0 ? addresses[0].id : null;
        this.isAddressLoading = false;
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.addresses = [];
        this.selectedAddressId = null;
        this.isAddressLoading = false;
        this.addressErrorMessage = 'Adres bilgileri yüklenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  toggleAddressForm(): void {
    this.isAddressFormVisible = !this.isAddressFormVisible;
    this.addressErrorMessage = '';
    this.addressSuccessMessage = '';
  }

  getPaymentMethodLabel(paymentMethod: string): string {
    const paymentLabels: Record<string, string> = {
      'Credit Card': 'Kredi Kartı',
      'Bank Transfer': 'Banka Havalesi',
      Cash: 'Kapıda Ödeme'
    };

    return paymentLabels[paymentMethod] ?? paymentMethod;
  }

  saveAddress(): void {
    this.addressErrorMessage = '';
    this.addressSuccessMessage = '';

    if (!this.currentUserId) {
      return;
    }

    if (!this.isAddressFormValid()) {
      this.addressErrorMessage = 'Adres başlığı, şehir, ilçe, adres detayı ve geçerli telefon alanlarını doldurmalısın.';
      return;
    }

    this.isAddressSaving = true;

    this.userAddressService.createUserAddress({
      ...this.newAddress,
      phone: this.normalizePhone(this.newAddress.phone)
    }).subscribe({
      next: (address) => {
        this.addresses = [...this.addresses, address];
        this.selectedAddressId = address.id;
        this.newAddress = this.createEmptyAddress();
        this.isAddressFormVisible = false;
        this.isAddressSaving = false;
        this.addressSuccessMessage = 'Adres başarıyla eklendi.';
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.isAddressSaving = false;
        this.addressErrorMessage = 'Adres eklenirken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  completeCheckout(): void {
    if (this.isOrderCreated) {
      return;
    }

    if (this.isGuestMode) {
      this.completeGuestCheckout();
      return;
    }

    this.completeAccountCheckout();
  }

  private completeAccountCheckout(): void {
    if (!this.currentUserId) {
      this.errorMessage = 'Siparişi hesabınla tamamlamak için giriş yapmalısın.';
      return;
    }

    if (!this.selectedAddressId) {
      this.errorMessage = 'Siparişi tamamlamak için teslimat adresi seçmelisin.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.createdOrder = null;

    this.orderService.createOrderFromCart(this.currentUserId, {
      userAddressId: this.selectedAddressId,
      paymentMethod: this.paymentMethod
    }).subscribe({
      next: (order) => {
        this.createdOrder = order;
        this.isLoading = false;
        this.successMessage = 'Siparişiniz ve ödeme bilginiz başarıyla oluşturuldu.';
        this.changeDetector.detectChanges();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.status === 400
          ? 'Sepetindeki bir ürünün stoğu yetersiz veya teslimat adresi geçersiz.'
          : 'Sipariş veya ödeme bilgisi oluşturulurken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  private completeGuestCheckout(): void {
    this.guestCart = this.localCartService.getCart();

    if (this.guestCart.items.length === 0) {
      this.errorMessage = 'Siparişi tamamlamak için sepetinde ürün olmalı.';
      return;
    }

    if (!this.isGuestFormValid()) {
      this.errorMessage = 'Misafir sipariş için ad, soyad, geçerli e-posta, geçerli telefon ve adres bilgilerini doldurmalısın.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.createdOrder = null;

    this.orderService.createGuestOrder({
      ...this.guestForm,
      phone: this.normalizePhone(this.guestForm.phone),
      paymentMethod: this.paymentMethod,
      items: this.guestCart.items.map((item) => ({
        productId: item.productId,
        quantity: item.quantity
      }))
    }).subscribe({
      next: (order) => {
        this.createdOrder = order;
        this.localCartService.clearCart();
        this.isLoading = false;
        this.successMessage = 'Misafir siparişiniz ve ödeme bilginiz başarıyla oluşturuldu.';
        this.changeDetector.detectChanges();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.status === 400
          ? 'Sepetindeki bir ürünün stoğu sipariş için yeterli değil.'
          : 'Misafir sipariş oluşturulurken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  private createEmptyAddress(): CreateUserAddress {
    return {
      title: '',
      city: '',
      district: '',
      addressDetail: '',
      phone: ''
    };
  }

  private createEmptyGuestForm(): GuestCheckoutForm {
    return {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      city: '',
      district: '',
      addressDetail: ''
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

  private isGuestFormValid(): boolean {
    return Boolean(
      this.guestForm.firstName.trim() &&
      this.guestForm.lastName.trim() &&
      this.isEmailValid(this.guestForm.email) &&
      this.isPhoneValid(this.guestForm.phone) &&
      this.guestForm.city.trim() &&
      this.guestForm.district.trim() &&
      this.guestForm.addressDetail.trim()
    );
  }

  private isEmailValid(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
  }

  private isPhoneValid(phone: string): boolean {
    return /^(?:5\d{9}|05\d{9})$/.test(this.normalizePhone(phone));
  }

  private normalizePhone(phone: string): string {
    return phone.replace(/\s+/g, '');
  }
}
