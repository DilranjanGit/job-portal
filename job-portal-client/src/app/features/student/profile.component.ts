
// src/app/features/student/profile/profile.component.ts
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { StudentService} from '../../core/services/student.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./registration.component.scss']
})
export class  StudentProfileComponent implements OnInit
{
    private route = inject(ActivatedRoute);
  private router = inject(Router);
  form: FormGroup;
  loading = false;
  saved = false;
  error = '';
  email : string | null = null; 

 ngOnInit(): void {
   
  // One-time read:
    this.email = null;
    this.route.queryParams.subscribe(params => {
       this.email = params['email'] || '';
     });

// Show it in the disabled control
    this.form.patchValue({ email: this.email }, { emitEvent: false });
    this.loadProfile(); 
  }
  constructor(private fb: FormBuilder, private api: StudentService) {
    this.form = this.fb.group({
      fullName: [{value: '', disabled: true}, [Validators.required, Validators.minLength(2)]],
      email: [{value: '', disabled: true}, [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+\d+-\d{10}$/)]], //  E.+91-XXXXXXXXXX  format
      location: ['', Validators.required],
      education: ['', Validators.required],
      skillsCsv: ['', [Validators.required, Validators.minLength(2)]]
    });
  

  }
  private loadProfile() {
    this.loading = true;
    const { email } = this.form.getRawValue();
    this.api.getStudentProfile(email).subscribe({
      next: (p) => {
        this.form.patchValue({
          fullName: p.fullName,
          email: p.email,
          phoneNumber: p.phoneNumber || '',
          location: p.location || '',
          education: p.education || '',
          skillsCsv: (p.skillsCsv || '')
        });
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  onSave() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = {
      fullName: this.form.get('fullName')?.value,
      email: this.form.get('email')?.value,
      phoneNumber: this.form.get('phoneNumber')?.value,
      education: this.form.get('education')?.value,
      location: this.form.get('location')?.value,
      skills: (this.form.get('skillsCsv')?.value || '')
        .split(',')
        .map((s: string) => s.trim())
        .filter(Boolean)
    };
    // show success or error message based on API response on success redirect to dashboard
    
    this.saved = false;
    this.error = '';
    this.api.updateStudentProfile(payload).subscribe({
      next: () => {
        alert('Profile updated successfully');
        // Redirect to dashboard
        this.router.navigate(['/dashboard'], { state: { email: payload.email } });
        this.saved = true;
      },
      error: (err) => {
        console.error('PUT /api/students/profile failed', err);
        const msg = err?.error?.message || err?.statusText || 'Update failed';
        this.error = `Failed to update profile: ${msg}`;
}
    }); 
  }
}
