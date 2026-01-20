
// src/app/features/dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

import { AuthService } from '../core/services/auth.service';
import { StudentService } from '../core/services/student.service';
import { CompanyService } from '../core/services/company.service';

interface DashboardState {
  email: string;
  role: string;
}

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class AdminDashboardComponent implements OnInit {
  
  userName = '';
  email = '';
  role= '';
  routeNotFound = false;

  constructor(
    private router: Router,
    private authService: AuthService,
    private studentService: StudentService,
    private companyService: CompanyService
  ) {
    

  }



  ngOnInit(): void {
    this.loadNavigationState();
    this.loadUserProfile();
  }

  private loadNavigationState(): void {
    
const navigation = this.router.getCurrentNavigation();
  const state = navigation?.extras.state as { email: string, role: string };

  this.email = state?.email || localStorage.getItem('email') || '';
  this.role  = state?.role  || localStorage.getItem('role')  || '';

  if (!this.email || !this.role) {
    console.warn("Missing state, redirecting to login...");
    this.router.navigate(['/login']);
    return;
  }
/*
    if (state) {
      this.email = state.email;
      this.role = state.role;
    } else {
      console.warn('No navigation state found, redirecting to login...');
      this.router.navigate(['/login']);
    }*/
  }

  private loadUserProfile(): void {
    if (!this.email || !this.role) return;

    switch (this.role) {
      case 'Admin':
        this.authService.GetUserDetails(this.email).subscribe(profile => {
          console.log('Admin Profile:', profile);
          this.mapProfile(profile.userName, profile.email);
        });
        break;

      case 'Student':
        this.studentService.getStudentProfile(this.email).subscribe(profile => {
          console.log('Student Profile:', profile);
          this.mapProfile(profile.fullName, profile.email);
        });
        break;

      case 'Company':
        this.companyService.getCompanyProfile(this.email).subscribe(profile => {
          console.log('Company Profile:', profile);
          this.mapProfile(profile.companyName, profile.email);
        });
        break;
    }
  }

  private mapProfile(name: string, email: string): void {
    this.userName = name;
    this.email = email;
  }

  logout(): void {
    localStorage.clear();
    sessionStorage.clear();
    this.router.navigate(['/login']);
  }
}
