
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule, FormGroup } from '@angular/forms';
import { StudentService } from '../../core/services/student.service';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-resume',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './resume.component.html'
})
export class StudentResumeComponent implements OnInit {

  form!: FormGroup;
  selectedFile: File | null = null;
  fileName = '';
  fileType = '';
  fileUrl: SafeResourceUrl | null = null;

  loading = false;
  saved = false;
  error = '';
  email: string = '';

  constructor(
    private fb: FormBuilder,
    private api: StudentService,
    private sanitizer: DomSanitizer,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {

    this.form = this.fb.group({
      resumeFile: [null, Validators.required],
      email: ['', Validators.required]
    });

    // Read email from query params
    this.route.queryParams.subscribe(params => {
      this.email = params['email'] || '';
      this.form.patchValue({ email: this.email });
    });

    
  this.route.queryParams.subscribe(params => {
      this.email = params['email'] || '';
      if (this.email) {
        this.loadResume();
      }
    });

  }
  
loadResume() {
    this.loading = true;

    this.api.downloadResume(this.email).subscribe({
      next: (response) => {
        this.loading = false;

        this.fileType = response.headers.get('Content-Type') || '';
        const contentDisposition = response.headers.get('Content-Disposition');

        this.fileName = this.extractFileName(contentDisposition);

        const blob = new Blob([response.body!], { type: this.fileType });
        const url = URL.createObjectURL(blob);

        this.fileUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
      },
      error: err => {
        this.loading = false;
        this.error = 'Failed to load resume';

      }
    });
  }
  
  extractFileName(header: string | null): string {
    if (!header) return 'resume';
    const match = header.match(/filename="?(.+)"?/);
    return match ? match[1] : 'resume';
  }
  
  download() {
    if (!this.fileUrl) return;

    const a = document.createElement('a');
    a.href = (this.fileUrl as any).changingThisBreaksApplicationSecurity;
    a.download = this.fileName;
    a.click();
  }


  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      this.selectedFile = null;
      this.form.patchValue({ resumeFile: null });
      return;
    }

    const file = input.files[0];
    this.selectedFile = file;

    // only store filename in form (not the file itself)
    this.form.patchValue({ resumeFile: file.name });
  }

  onSubmit() {
    if (!this.form.valid || !this.selectedFile) {
      alert('Please select a valid file first.');
      return;
    }

    const formData = new FormData();
    formData.append('resumeFile', this.selectedFile, this.selectedFile.name);
    formData.append('email', this.form.value.email);

    this.api.uploadStudentResume(formData).subscribe({
      next: () => {
        alert('Resume uploaded successfully');
        this.router.navigate(['/dashboard'], {
          state: { email: this.email }
        });
        this.saved = true;
      },
      error: err => {
        console.error('Upload failed', err);
        this.error = `Failed to upload resume: ${err?.error?.message || err?.statusText}`;
      }
    });
  }
}
