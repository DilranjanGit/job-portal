
// src/app/features/dashboard.component.ts
import { Component,OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { StudentService } from '../core/services/student.service';
import { CompanyService } from '../core/services/company.service';

@Component({
  standalone: true,
  selector: 'app-admin-dashboard',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'] 
})
export class AdminDashboardComponent implements OnInit {
  userName: string ='';
  email: string ='';
  role: string ='';

  constructor(private router: Router, private authService: AuthService, private studentService: StudentService, private companyService: CompanyService) {
  
   // You can use the email from the state here
const navigation = this.router.getCurrentNavigation();
  const state = navigation?.extras.state as { email: string, role: string };
  //console.log(state?.email); // 'student@example.com'
    this.email = state?.email || '';
    this.role = state?.role || '';
  }
ngOnInit(): void {
   // const email = this.email;
//call auth service to fetch User profile 
 if (this.role=='Admin')
 {
this.authService.GetUserDetails(this.email).subscribe(profile => {
    console.log('User Profile:', profile);
    this.userName = profile.userName;
    this.email = profile.email;
  });
}
else if (this.role=='Student')
{
  this.studentService.getStudentProfile(this.email).subscribe(profile => {
    console.log('User Profile:', profile);
    this.userName = profile.fullName;
    this.email = profile.email;
  });
}
else if (this.role=='Company')
{
  this.companyService.getCompanyProfile(this.email).subscribe(profile => {
    console.log('User Profile:', profile);
    this.userName = profile.companyName;
    this.email = profile.email;
  });
}
}

  }
