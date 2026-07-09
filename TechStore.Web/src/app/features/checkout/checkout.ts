import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../core/services/order.service';
import { UserAddressService } from '../../core/services/user-address.service';
import { Order } from '../../models/order.model';
import { CreateUserAddress, UserAddress } from '../../models/user-address.model';

@Component({
  selector: 'app-checkout',
  imports: [CommonModule, FormsModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css'
})
export class CheckoutComponent implements OnInit {
  currentUserId = 2;
  selectedAddressId: number | null = null;
  addresses: UserAddress[] = [];

  newAddress: CreateUserAddress = this.createEmptyAddress();
  isAddressFormVisible = false;

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
    private orderService: OrderService,
    private userAddressService: UserAddressService,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getUserAddresses();
  }

  get selectedAddress(): UserAddress | undefined {
    return this.addresses.find((address) => address.id === this.selectedAddressId);
  }

  getUserAddresses(): void {
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

    if (!this.isAddressFormValid()) {
      this.addressErrorMessage = 'Adres başlığı, şehir, ilçe ve adres detayı alanlarını doldurmalısın.';
      return;
    }

    this.isAddressSaving = true;

    this.userAddressService.createUserAddress({
      ...this.newAddress,
      userId: this.currentUserId
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
        this.successMessage = 'Siparişiniz ve ödemeniz başarıyla oluşturuldu.';
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Sipariş veya ödeme oluşturulurken bir hata oluştu.';
        this.changeDetector.detectChanges();
      }
    });
  }

  private createEmptyAddress(): CreateUserAddress {
    return {
      userId: this.currentUserId,
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
      this.newAddress.addressDetail.trim()
    );
  }
}
