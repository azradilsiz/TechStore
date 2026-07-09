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
  userId: number;
  userName: string;
  userAddressId: number;
  addressTitle: string;
  orderDate: string;
  totalPrice: number;
  status: string;
  items: OrderItem[];
  hasPayment: boolean;
}

export interface CreateOrderFromCart {
  userAddressId: number;
  paymentMethod: string;
}
