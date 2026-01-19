
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CompanyService {
  private http = inject(HttpClient);
  private companyUrl = `${environment.apiUrl}/api/companies`;

  // Register Company
  registerCompany(payload: { companyName: string, location: string, websiteUrl: string; email: string; password: string; }): Observable<any> {
    return this.http.post<{ token: string; role: string }>(`${this.companyUrl}/register`, payload);
  }

  //Get Company Profile
  getCompanyProfile(email: string):Observable<any>{
    return this.http.get<{ token: String; role: string }>(`${this.companyUrl}/profile`, {params: {email}});
  }

  //Update Company Profile
  updateCompanyProfile(payload: { email: string; location: string; website: string; }): Observable<any> {
    return this.http.put<{ success: boolean }>(`${this.companyUrl}/profile`, payload);
  }

  //Post Jobs
  postJobs(payload: { companyEmail:string; title: string; description: string; location : string; skills: string; salary: string}): Observable<any>{
    return this.http.post<{ success:boolean}>(`${this.companyUrl}/jobs`,payload);
  }
  
  }

 
