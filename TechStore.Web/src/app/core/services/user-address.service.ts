import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, timeout } from 'rxjs';
import { CreateUserAddress, UserAddress } from '../../models/user-address.model';

@Injectable({
  providedIn: 'root'
})
export class UserAddressService {
  private readonly apiUrl = 'http://localhost:5050/api/UserAddresses';

  constructor(private http: HttpClient) {}

  getUserAddressesByUserId(userId: number): Observable<UserAddress[]> {
    return this.http.get<UserAddress[]>(`${this.apiUrl}/user/${userId}`).pipe(
      timeout(10000)
    );
  }

  createUserAddress(dto: CreateUserAddress): Observable<UserAddress> {
    return this.http.post<UserAddress>(this.apiUrl, dto).pipe(
      timeout(10000)
    );
  }
}
