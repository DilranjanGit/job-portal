// src/app/features/student/resume.component.ts
import { CommonModule } from '@angular/common';
import { Component,OnInit } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule, FormGroup } from '@angular/forms';
import { StudentService} from '../../core/services/student.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-resume',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './resume.component.html'
  //styleUrls: ['./resume.component.scss']
})
export class StudentResumeComponent implements OnInit
{
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
// Show it in the hidden control
    this.form.patchValue({ email: this.email }, { emitEvent: false });

   };

   constructor(private fb: FormBuilder, private api: StudentService , private route: ActivatedRoute, private router: Router ) {
     this.form = this.fb.group({
       resumeFile: [null, Validators.required] // Add validators as needed
     });
 }
   byteArray: Uint8Array | null = null;
   fileName: string = '';
   contentType: string = '';
   file : File | null = null;
  // Handle file input change event
 
onFileChange(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files.length > 0) {
    this.file = input.files[0];
    this.fileName = this.file.name;
    this.contentType = this.file.type;

    const reader = new FileReader();
    reader.onload = () => {
      this.byteArray = new Uint8Array(reader.result as ArrayBuffer);
      console.log('File loaded as byte array:', this.byteArray);
    };
    reader.readAsArrayBuffer(this.file); // ✅ Use ArrayBuffer for binary
  }
}

  // handle file upload on form submission
 
onSubmit() {
  if (!this.fileName || !this.byteArray) {
    alert('Please select a file first.');
    return;
  }

  const formData = new FormData();
  formData.append('fileName', this.fileName);
  formData.append('contentType', this.contentType);
  formData.append('fileSize', this.byteArray.length.toString());
  formData.append('fileData', new Blob([this.byteArray], { type: 'application/octet-stream' }));
  formData.append('email', this.email || '');
  formData.append('resumeFile', new Blob([this.byteArray], { type: this.contentType }), this.fileName);
  const { email } = this.form.getRawValue();
  //const test = this.email || '';
  this.api.uploadStudentResume(formData).subscribe({
    next: () => {
      alert('Resume uploaded successfully');
      this.router.navigate(['/student/dashboard'], {   state: { email: this.email || '' } });
      this.saved = true;
    },
    error: (err) => {
      console.error('Upload failed', err);
      this.error = `Failed to update profile: ${err?.error?.message || err?.statusText}`;
    }
  });
}


}
