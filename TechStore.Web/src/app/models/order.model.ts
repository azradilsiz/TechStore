export interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Order {
  id: number;
  orderNumber: string;
  userId: number | null;
  userName: string;
  userEmail: string;
  userAddressId: number | null;
  addressTitle: string;
  guestFullName: string;
  guestEmail: string;
  guestPhone: string;
  guestAddress: string;
  orderDate: string;
  totalPrice: number;
  status: string;
  items: OrderItem[];
  hasPayment: boolean;
  paymentId: number | null;
  paymentMethod: string;
  paymentStatus: string;
}

export interface CreateOrderFromCart {
  userAddressId: number;
  paymentMethod: string;
}

export interface CreateGuestOrder {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  city: string;
  district: string;
  addressDetail: string;
  paymentMethod: string;
  items: CreateOrderItem[];
}

export interface CreateOrderItem {
  productId: number;
  quantity: number;
}
