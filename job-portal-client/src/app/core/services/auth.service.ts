
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/auth`;

  // Register Student
  registerStudent(payload: { email: string; password: string; fullName: string; education: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/register/student`, payload);
  }

  // Register Company
  registerCompany(payload: { email: string; password: string; companyName: string; location: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/register/company`, payload);
  }

  // Login
  login(email: string, password: string): Observable<{ token: string; role: string }> {
    return this.http.post<{ token: string; role: string }>(`${this.baseUrl}/login`, { email, password });
  }

  // Token & Role Management
  setAuth(token: string, role: string) {
    localStorage.setItem('token', token);
    localStorage.setItem('role', role);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getRole(): string | null {
    return localStorage.getItem('role');
  }

  logout() {
    localStorage.clear();
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
