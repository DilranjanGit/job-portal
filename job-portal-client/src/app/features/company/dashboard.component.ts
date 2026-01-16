
// src/app/features/company/dashboard.component.ts
import { Component,OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'] 
})
export class CompanyDashboardComponent implements OnInit {
  userName: string ='';
  email: string ='';

  constructor(private router: Router) {

   // You can use the email from the state here
   const navigation = this.router.getCurrentNavigation();
   const state = navigation?.extras.state as { email: string };
   //console.log(state?.email); // 'student@example.com'
   this.email = state?.email || '';
  }
  ngOnInit() {

  const email = this.email;
  //call student service to fetch profile or other data using the email
  /* this.studentService.getStudentProfile(email).subscribe(profile => {
    console.log('Student Profile:', profile);
    this.userName = profile.fullName;
    this.email = profile.email;
  });*/
  }
}
