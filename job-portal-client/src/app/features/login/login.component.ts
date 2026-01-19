
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [RouterLink, ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = false;
  error = '';
  showPassword = false;

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  login(): void {
    if (this.form.invalid) return;

    this.loading = true;
    this.error = '';

    const { email, password } = this.form.value;

    this.auth.login(email!, password!).subscribe({
      next: (res) => {
        this.handleLoginSuccess(res.token, res.role, email!);
      },
      error: () => {
        this.error = 'Invalid credentials';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  private handleLoginSuccess(token: string, role: string, email: string): void {
    this.auth.setAuth(token, role);
  
    // Store login details locally
    localStorage.setItem('email', email);
    localStorage.setItem('role', role);

    // Unified dashboard navigation
    this.router.navigate(['/dashboard']);
     // , {
      //state: { email, role }
    //});
  }
}
