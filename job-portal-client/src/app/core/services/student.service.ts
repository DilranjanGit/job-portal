export interface Job{
id: number,
companyProfileID: number,

}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpResponse } from '@angular/common/http';
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
  
  //Update Student Profile
  updateStudentProfile(payload: { email: string; education: string; phoneNumber: string; location: string; skills: string[]}): Observable<any> {
    return this.http.put<{ success: boolean }>(`${this.studentUrl}/profile`, payload);
  }

  // Upload Student Resume
  uploadStudentResume(resumeFile: FormData): Observable<any> {
    return this.http.post(`${this.studentUrl}/resume`, resumeFile);
  }

  //Download Student Resume
  downloadResume(email: string): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.studentUrl}/resume?email=${email}`, {
      responseType: 'blob',
      observe: 'response'
    });
  }


  //Get All Jobs match with student skills ans location
  getAllJobs(email: string):Observable<any>{
    return this.http.get<Job[]>(`${this.studentUrl}/jobs`,{params: {email}});
  }
 
  // Apply job
  ApplyJob( payload: {studentEmail: string; jobId: string} ):Observable<any>{
     return this.http.post<{success: boolean }>(`${this.studentUrl}/apply`, payload);
  }
// Get all Students
    getAllStudents(): Observable<any[]> {
      return this.http.get<any[]>(`${this.studentUrl}/allProfiles`);
    }

    // Update Student status
    updateStudentStatus(companyId: number, isActive: boolean): Observable<any> {
      return this.http.put(`${this.studentUrl}/status`, { companyId, isActive });
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
