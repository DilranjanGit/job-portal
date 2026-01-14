
// src/app/features/student/dashboard/dashboard.component.ts
import { Component,OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { StudentService } from '../../core/services/student.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class  StudentDashboardComponent  implements OnInit {
  userName: string ='';
  email: string ='';

  constructor(private router: Router, private studentService: StudentService) {
  
   // You can use the email from the state here
const navigation = this.router.getCurrentNavigation();
  const state = navigation?.extras.state as { email: string };
  //console.log(state?.email); // 'student@example.com'
    this.email = state?.email || '';
}
  ngOnInit() {

  const email = this.email;
//call student service to fetch profile or other data using the email
  this.studentService.getStudentProfile(email).subscribe(profile => {
    console.log('Student Profile:', profile);
    this.userName = profile.fullName;
    this.email = profile.email;
  });
  }
}