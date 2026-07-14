import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, forkJoin, map, Observable, of, switchMap, tap, timeout } from 'rxjs';
import { AuthResponseDto, ChangePasswordDto, LoginDto, RegisterDto } from '../../models/auth.model';
import { CartService } from './cart.service';
import { LocalCartService } from './local-cart.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5050/api/Auth';
  private readonly storageKey = 'techStoreUser';

  private currentUserSubject = new BehaviorSubject<AuthResponseDto | null>(this.getStoredUser());
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private cartService: CartService,
    private localCartService: LocalCartService
  ) {}

  login(dto: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, dto).pipe(
      timeout(10000),
      tap((user) => this.setCurrentUser(user)),
      switchMap((user) => this.mergeGuestCartToUserCart(user))
    );
  }

  register(dto: RegisterDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/register`, dto).pipe(
      timeout(10000),
      tap((user) => this.setCurrentUser(user)),
      switchMap((user) => this.mergeGuestCartToUserCart(user))
    );
  }

  changePassword(dto: ChangePasswordDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/change-password`, dto).pipe(
      timeout(10000)
    );
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.currentUserSubject.next(null);
  }

  getCurrentUser(): AuthResponseDto | null {
    return this.currentUserSubject.value;
  }

  getCurrentUserId(): number | null {
    return this.currentUserSubject.value?.userId ?? null;
  }

  isLoggedIn(): boolean {
    return this.currentUserSubject.value !== null;
  }

  isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user?.userTypeName.toLowerCase() === 'admin' || user?.userTypeId === 1;
  }

  private setCurrentUser(user: AuthResponseDto): void {
    localStorage.setItem(this.storageKey, JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private getStoredUser(): AuthResponseDto | null {
    const userJson = localStorage.getItem(this.storageKey);

    if (!userJson) {
      return null;
    }

    return JSON.parse(userJson) as AuthResponseDto;
  }

  private mergeGuestCartToUserCart(user: AuthResponseDto): Observable<AuthResponseDto> {
    const guestCart = this.localCartService.getCart();

    if (guestCart.items.length === 0) {
      return of(user);
    }

    const requests = guestCart.items.map((item) =>
      this.cartService.addItemToCart({
        userId: user.userId,
        productId: item.productId,
        quantity: item.quantity
      })
    );

    return forkJoin(requests).pipe(
      tap(() => this.localCartService.clearCart()),
      map(() => user),
      catchError(() => of(user))
    );
  }
}
