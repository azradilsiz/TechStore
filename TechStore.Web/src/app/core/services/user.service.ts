import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, timeout } from 'rxjs';
import { User, UserWriteDto } from '../../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly apiUrl = 'http://localhost:5050/api/Users';

  constructor(private http: HttpClient) {}

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl).pipe(timeout(10000));
  }

  updateUser(userId: number, dto: UserWriteDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${userId}`, dto).pipe(timeout(10000));
  }

  deleteUser(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${userId}`).pipe(timeout(10000));
  }
}
