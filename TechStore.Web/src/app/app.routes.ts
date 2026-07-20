import { Routes } from '@angular/router';
import { CartComponent } from './features/cart/cart';
import { CheckoutComponent } from './features/checkout/checkout';
import { HomeComponent } from './features/home/home';
import { OrdersComponent } from './features/orders/orders';
import { ProductDetailComponent } from './features/product-detail/product-detail';
import { ProductsComponent } from './features/products/products';
import { ForgotPasswordComponent } from './features/forgot-password/forgot-password';
import { LoginComponent } from './features/login/login';
import { ProfileComponent } from './features/profile/profile';
import { RegisterComponent } from './features/register/register';
import { adminGuard } from './core/guards/admin.guard';
import { authGuard } from './core/guards/auth.guard';
import { AdminComponent } from './features/admin/admin';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard';
import { AdminProductsComponent } from './features/admin/admin-products/admin-products';
import { AdminOrdersComponent } from './features/admin/admin-orders/admin-orders';
import { AdminUsersComponent } from './features/admin/admin-users/admin-users';

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
    component: OrdersComponent,
    canActivate: [authGuard]
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [authGuard]
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'forgot-password',
    component: ForgotPasswordComponent
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  {
    path: 'admin',
    component: AdminComponent,
    canActivate: [adminGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard'
      },
      {
        path: 'dashboard',
        component: AdminDashboardComponent
      },
      {
        path: 'products',
        component: AdminProductsComponent
      },
      {
        path: 'orders',
        component: AdminOrdersComponent
      },
      {
        path: 'users',
        component: AdminUsersComponent
      }
    ]
  },
];
