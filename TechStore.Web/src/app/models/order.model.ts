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
  userAddressId: number;
  orderDate: string;
  totalAmount: number;
  status: string;
  items: OrderItem[];
}

export interface CreateOrderFromCart {
  userAddressId: number;
}