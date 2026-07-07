export interface Payment {
  id: number;
  orderId: number;
  amount: number;
  paymentMethod: string;
  paymentStatus: string;
  paymentDate: string;
}

export interface CreatePayment {
  paymentMethod: string;
}