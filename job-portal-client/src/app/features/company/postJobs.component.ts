
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CompanyService } from '../../core/services/company.service';
import {ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-postJobs',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './postJobs.component.html',
  styleUrls: ['./postJobs.component.scss']
})
export class PostJobsComponent implements OnInit{
  registrationForm: FormGroup;
  showPassword = false;
  loading = false;
  saved = false;
  error = '';
  email : string | null = null; 

  ngOnInit(): void {
      
  }

  constructor(private fb: FormBuilder, private companyServices: CompanyService, private router: Router, private route: ActivatedRoute) {
    this.registrationForm = this.fb.group({
      companyName: [{value:'',disabled:true}, [Validators.required, Validators.minLength(2)]],
      email: [{value:'',disabled:true}, [Validators.required, Validators.email]],
      title: ['', [Validators.required, Validators.minLength(2)]],
      description: ['', [Validators.required, Validators.minLength(2)]],
      location: ['', [Validators.required, Validators.minLength(2)]],
      skillsCsv: ['', [Validators.required, Validators.minLength(2)]],
      salary: [''],

    });
    
    this.route.queryParams.subscribe(params => {
       this.email = params['email'] || '';
     });

// Show it in the disabled control
    this.registrationForm.patchValue({ email: this.email }, { emitEvent: false });
    this.loadProfile(); 
  }

  get form() { return this.registrationForm.controls; }

  private clearForm()
  {
    this.registrationForm = this.fb.group({
      title: '',
      description: '',
      location:'',
      skillsCsv: '',
      salary: ''
  });
}

  private loadProfile() {
    this.loading = true;
    const { email } = this.registrationForm.getRawValue();
    this.companyServices.getCompanyProfile(email).subscribe({
      next: (p) => {
        this.registrationForm.patchValue({
          companyName: p.companyName,
          email: p.email,
          //title: p.title,
          //description: p.description,
          location: p.location,
          skillsCsv: p.skillsCsv,
          //salary: p.salary
        });
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  onSubmit() {
    if (this.registrationForm.invalid) {
      this.registrationForm.markAllAsTouched();
      alert('Please fill out information correctly.');
      return;
    }

    const payload = {
      companyName: this.form['companyName'].value,
      companyEmail: this.form['email'].value,
      title: this.form['title'].value,
      description: this.form['description'].value,
      skills: this.form['skillsCsv'].value,
      location: this.form['location'].value,
      salary: this.form['salary'].value
      
    };

    this.companyServices.postJobs(payload).subscribe({
      next: () => {
        alert('Job Posted successful!');
        //this.registrationForm.reset();
        this.loadProfile();
        this.clearForm();
        // redirect to login page here
        this.router.navigate(['/dashboard']);   
      },
      error: (err) => alert('Error: ' + (err?.error?.message || err?.error || err.message || 'Unknown error'))
    });
  }
}
