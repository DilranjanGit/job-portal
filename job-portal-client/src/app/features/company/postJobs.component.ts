
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CompanyService } from '../../core/services/company.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-postJobs',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './postJobs.component.html',
  styleUrls: ['./postJobs.component.scss']
})
export class PostJobsComponent {
  registrationForm: FormGroup;
  showPassword = false;

  constructor(private fb: FormBuilder, private services: CompanyService, private router: Router) {
    this.registrationForm = this.fb.group({
      companyName: [{value:'',disabled:true}, [Validators.required, Validators.minLength(2)]],
      email: [{value:'',disabled:true}, [Validators.required, Validators.email]],
      title: ['', [Validators.required, Validators.minLength(2)]],
      description: ['', [Validators.required, Validators.minLength(2)]],
      location: ['', [Validators.required, Validators.minLength(2)]],
      skillsCsv: ['', [Validators.required, Validators.minLength(2)]],
      salary: [''],
    });
  }

  get form() { return this.registrationForm.controls; }

  onSubmit() {
    if (this.registrationForm.invalid) {
      this.registrationForm.markAllAsTouched();
      alert('Please fill out information correctly.');
      return;
    }

    const payload = {
      companyName: this.form['companyName'].value,
      email: this.form['email'].value,
      title: this.form['title'].value,
      description: this.form['description'].value,
      skillsCsv: this.form['skillsCsv'].value,
      location: this.form['location'].value,
      salary: this.form['salary'].value
      
    };

    this.services.postJobs(payload).subscribe({
      next: () => {
        alert('Registration successful!');
        this.registrationForm.reset();
        // redirect to login page here
        this.router.navigate(['/login']);   
      },
      error: (err) => alert('Error: ' + (err?.error?.message || err?.error || err.message || 'Unknown error'))
    });
  }
}
