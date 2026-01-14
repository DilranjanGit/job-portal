
// src/app/features/student/profile/profile.component.ts
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { StudentService} from '../../core/services/student.service';
import { ActivatedRoute } from '@angular/router';

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
  form: FormGroup;
  loading = false;
  saved = false;
  error = '';
  emailfromQueryParam : string | null = null; 

 ngOnInit(): void {
    // One-time read:
    this.emailfromQueryParam = this.route.snapshot.queryParamMap.get('email');
  }
  constructor(private fb: FormBuilder, private api: StudentService) {
    this.form = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      location: [''], // city/state/country
      skills: ['']    // comma-separated e.g., "Angular, TypeScript"
    });
    
 this.route.queryParamMap.subscribe(q => {
    const email = q.get('email');
    if (email) {
      this.form.patchValue({ email });
      this.loadProfile();
      //console.log(fullName);
    }
  });   
 
  }
  private loadProfile() {
    this.loading = true;
    const { email } = this.form.value;
    this.api.getStudentProfile(email).subscribe({
      next: (p) => {
        this.form.patchValue({
          fullName: p.fullName,
          email: p.email,
          phone: p.phone || '',
          location: p.location || '',
          skills: (p.skills || []).join(', ')
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
    /*
    const payload: StudentProfile = {
      fullName: this.form.get('fullName')?.value,
      email: this.form.get('email')?.value,
      phone: this.form.get('phone')?.value,
      location: this.form.get('location')?.value,
      skills: (this.form.get('skills')?.value || '')
        .split(',')
        .map((s: string) => s.trim())
        .filter(Boolean)
    };

    this.saved = false;
    this.error = '';
    this.api.updateProfile(payload).subscribe({
      next: () => (this.saved = true),
      error: (err) => (this.error = err?.error || err.message || 'Failed to save')
    }); */
  }
}
