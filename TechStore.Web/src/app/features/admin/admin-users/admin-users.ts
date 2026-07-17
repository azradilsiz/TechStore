import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../models/user.model';

@Component({ selector: 'app-admin-users', imports: [CommonModule, FormsModule], templateUrl: './admin-users.html', styleUrl: './admin-users.css' })
export class AdminUsersComponent implements OnInit {
  users: User[] = [];
  loading = true;
  errorMessage = '';
  searchTerm = '';
  constructor(private userService: UserService, private authService: AuthService, private cdr: ChangeDetectorRef) {}
  ngOnInit(): void { this.loadUsers(); }
  get filteredUsers(): User[] { const q=this.searchTerm.trim().toLocaleLowerCase('tr-TR'); return !q?this.users:this.users.filter(u=>`${u.firstName} ${u.lastName} ${u.email} ${u.userName}`.toLocaleLowerCase('tr-TR').includes(q)); }
  loadUsers(): void { this.loading=true; this.userService.getUsers().subscribe({next:users=>{this.users=users;this.loading=false;this.cdr.detectChanges();},error:()=>{this.errorMessage='Kullanıcılar yüklenemedi.';this.loading=false;this.cdr.detectChanges();}}); }
  changeRole(user: User, userTypeId: number): void { const previous=user.userTypeId; user.userTypeId=userTypeId; this.userService.updateUser(user.id,{userTypeId,userName:user.userName,firstName:user.firstName,lastName:user.lastName,email:user.email}).subscribe({error:()=>{user.userTypeId=previous;this.errorMessage='Kullanıcı rolü güncellenemedi.';this.cdr.detectChanges();}}); }
  remove(user: User): void { if(user.id===this.authService.getCurrentUserId()){this.errorMessage='Kendi yönetici hesabınızı silemezsiniz.';return;} if(!confirm(`${user.firstName} ${user.lastName} kullanıcısını pasifleştirmek istiyor musunuz?`))return; this.userService.deleteUser(user.id).subscribe({next:()=>this.loadUsers(),error:()=>{this.errorMessage='Kullanıcı pasifleştirilemedi.';this.cdr.detectChanges();}}); }
}
