import { Routes } from '@angular/router';
import { CartComponent } from './features/cart/cart';
import { Products } from './features/products/products';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'products',
    pathMatch: 'full'
  },
  {
    path: 'products',
    component: Products
  },
  {
    path: 'cart',
    component: CartComponent
  }
];