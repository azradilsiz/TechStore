export interface CartItem {
    id: number;
    productId: number;
    productName: string;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
}

export interface Cart {
    id: number;
    userId: number;
    items: CartItem[];
    totalPrice: number;
}

export interface AddCartItem {
  productId: number;
  quantity: number;
}


export interface UpdateCartItem {
  quantity: number;
}
