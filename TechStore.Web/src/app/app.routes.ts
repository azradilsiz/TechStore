import { Routes } from '@angular/router';
import { CartComponent } from './features/cart/cart';
import { CheckoutComponent } from './features/checkout/checkout';
import { HomeComponent } from './features/home/home';
import { OrdersComponent } from './features/orders/orders';
import { ProductDetailComponent } from './features/product-detail/product-detail';
import { ProductsComponent } from './features/products/products';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'products',
    component: ProductsComponent
  },
  {
    path: 'products/:productId',
    component: ProductDetailComponent
  },
  {
    path: 'cart',
    component: CartComponent
  },
  {
    path: 'checkout',
    component: CheckoutComponent
  },
  {
    path: 'orders',
    component: OrdersComponent
  }
];
