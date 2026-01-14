import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router'; 

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
    password: ['', [Validators.required,]],
  });

  login() {
    if (this.form.invalid) return;
    this.loading = true;
    this.error = '';

    const { email, password } = this.form.value;
    this.auth.login(email!, password!).subscribe({
      next: (res) => {
        // If backend role is not 'Student', optionally block here
        this.auth.setAuth(res.token, res.role);
        if (res.role === 'Student') {
          this.router.navigate(['/student/dashboard'], { state: { email } });
        } else if (res.role === 'Company') {
          this.router.navigate(['/company/dashboard'], { state: { email } });
        } else if (res.role === 'Admin') {
          this.router.navigate(['/admin/dashboard'], { state: { email } });
        }
        else {
          this.error = 'Unauthorized role';
          alert('Unauthorized role');
        }

      },
      error: (err) => {
        this.error = 'Invalid credentials';
        this.loading = false
      },
      complete: () => (this.loading = false)
    });
  }
}
