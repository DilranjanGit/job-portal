
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CompanyService } from '../../core/services/company.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class CompanyRegistrationComponent {
  registrationForm: FormGroup;
  showPassword = false;

  constructor(private fb: FormBuilder, private services: CompanyService, private router: Router) {
    this.registrationForm = this.fb.group({
      companyName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{6,}$')]],
      location: ['', [Validators.required, Validators.minLength(2)]],
      website: ['', [Validators.required, Validators.pattern('https?://.+')]] // accept valid URL google.com or http://google.com or https://google.com
    });
  }
  goToLogin()
  {
    this.router.navigate(['/login']);
  }

  get form() { return this.registrationForm.controls; }

  onSubmit() {
    if (this.registrationForm.invalid) {
      this.registrationForm.markAllAsTouched();
      alert('Please fill out the form correctly.');
      return;
    }

    const payload = {
      companyName: this.form['companyName'].value,
      location: this.form['location'].value,
      websiteUrl: this.form['website'].value,
      email: this.form['email'].value,
      password: this.form['password'].value
    };

    this.services.registerCompany(payload).subscribe({
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
