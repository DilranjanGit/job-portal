
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StudentService } from '../../core/services/student.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  standalone: true,                            // ✅ required
  imports: [CommonModule, ReactiveFormsModule],// ✅ CommonModule for *ngIf, ReactiveFormsModule for formGroup etc.
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent {
  registrationForm: FormGroup;
  showPassword = false;

  constructor(private fb: FormBuilder, private services: StudentService, private router: Router) {
    this.registrationForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  get form() { return this.registrationForm.controls; }

  onSubmit() {
    if (this.registrationForm.invalid) {
      this.registrationForm.markAllAsTouched();
      alert('Please fill out the form correctly.');
      return;
    }

    const payload = {
      fullName: this.form['fullName'].value,
      email: this.form['email'].value,
      password: this.form['password'].value
    };

    this.services.registerStudent(payload).subscribe({
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
