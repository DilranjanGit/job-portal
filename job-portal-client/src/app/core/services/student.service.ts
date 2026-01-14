
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private http = inject(HttpClient);
  private studentUrl = `${environment.apiUrl}/api/students`;
  

  // Register Student
  registerStudent(payload: { email: string; password: string; fullName: string}): Observable<any> {
    return this.http.post<{ token: string; role: string }>(`${this.studentUrl}/register`, payload);
  }
 
  //Get Student Profile
  getStudentProfile(email: string):Observable<any>{
    return this.http.get<{ token: String; role: string }>(`${this.studentUrl}/profile`, {params: {email}});
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
