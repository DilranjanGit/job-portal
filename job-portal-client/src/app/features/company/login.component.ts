
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-company-login',
  imports: [ReactiveFormsModule, CommonModule],
  template: `
    <section class="login">
      <h2>Company Login</h2>

      <form [formGroup]="form" (ngSubmit)="login()">
        <div class="form-row">
          <label>Email</label>
          <input type="email" formControlName="email" placeholder="hr@company.com" />
          <small class="error" *ngIf="form.controls.email.touched && form.controls.email.invalid">
            Valid email is required
          </small>
        </div>

        <div class="form-row">
          <label>Password</label>
          <input type="password" formControlName="password" placeholder="••••••••" />
          <small class="error" *ngIf="form.controls.password.touched && form.controls.password.invalid">
            Password is required
          </small>
        </div>

        <button type="submit" [disabled]="form.invalid || loading">
          {{ loading ? 'Logging in...' : 'Login' }}
        </button>

        <p class="error" *ngIf="error">{{ error }}</p>
      </form>
    </section>
  `,
  styles: [`
    .login { max-width: 380px; margin: 32px auto; padding: 24px; border: 1px solid #ddd; border-radius: 8px; }
    .form-row { margin-bottom: 12px; display: flex; flex-direction: column; }
    label { font-weight: 600; margin-bottom: 6px; }
    input { padding: 8px; border: 1px solid #bbb; border-radius: 4px; }
    button { margin-top: 12px; width: 100%; padding: 10px; }
    .error { color: #b00020; font-size: 12px; margin-top: 6px; }
  `]
})
export class CompanyLoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = false;
  error = '';

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  login() {
    if (this.form.invalid) return;
    this.loading = true;
    this.error = '';

    const { email, password } = this.form.value;
    this.auth.login(email!, password!).subscribe({
      next: (res) => {
        // If backend role is not 'Company', optionally block here
        this.auth.setAuth(res.token, res.role);
        this.router.navigate(['/company/dashboard']);
      },
      error: () => {
        this.error = 'Invalid credentials';
        this.loading = false;
      },
      complete: () => (this.loading = false)
    });
  }
}
