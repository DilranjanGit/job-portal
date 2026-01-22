
export interface Job {
  id: number;
  title: string;
  location: string;
  applications: any[];   // <-- important
}

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CompanyService } from '../../core/services/company.service';
import { StudentService } from '../../core/services/student.service'; 
import { CommonModule } from '@angular/common';
import { SafeResourceUrl,DomSanitizer } from '@angular/platform-browser';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-interview',
  templateUrl: './Interview.component.html',
  styleUrls: ['./interview.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class InterviewComponent {

  jobs: Job[] = [];              // <-- Correct typing
  expandedJob: Job | null = null;
  showModal = false;
  scheduleForm: FormGroup;
  modes = ['In-Person', 'Online', 'Phone'];
  companyEmail: string = "";
  studentEmail: string= "";
  fileName = '';
  fileType = '';
  fileUrl: SafeResourceUrl | null = null;
  loading = true;

  constructor(
    private http: HttpClient,
    private fb: FormBuilder,
    private _companyService: CompanyService,
    private _studentService: StudentService,
    private sanitizer: DomSanitizer
  ) {
    this.scheduleForm = this.fb.group({
      id:0,
      jobApplicationId: [0, Validators.required],
      interviewDate: ['', Validators.required],
      mode: [0, Validators.required],
      locationOrLink: ['', Validators.required]
    });

    this.fetchJobs();
  }

//#region  Download resume Feature
  downloadResume(app: any) {
    
     this.studentEmail= app.student.email
     this._companyService.downloadResume(this.studentEmail)
    .pipe(finalize(() => {
        this.loading = true; // re-enable button ALWAYS (success or error)
      }))
      .subscribe({
      next: (response) => {
        this.loading = false;

        this.fileType = response.headers.get('Content-Type') || '';
        const contentDisposition = response.headers.get('Content-Disposition');

        this.fileName = this.extractFileName(contentDisposition);

        const blob = new Blob([response.body!], { type: this.fileType });
        const url = URL.createObjectURL(blob);

        this.fileUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
        const a = document.createElement('a');
        a.href = (this.fileUrl as any).changingThisBreaksApplicationSecurity;
        a.download = this.fileName;
        a.click();
      },
      error: err => {
        alert('Failed to download resume');
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
//#endregion 

  fetchJobs() {
    this.companyEmail = localStorage.getItem("email") || "";

    this._companyService.getJobApplications(this.companyEmail).subscribe({
      next: (data: any[]) => {
        console.log(data);

        // Ensure type safety
        this.jobs = data.map((job: any) => ({
          id: job.id,
          title: job.title,
          location: job.location,
          applications: job.applications || []
        }));

        console.log(this.jobs);
      },
      error: () => {
        this.jobs = [];
      }
    });
  }

  toggleApplicants(job: Job) {
    this.expandedJob = this.expandedJob === job ? null : job;
  }

  openScheduleModal(app: any) {
    this.scheduleForm.reset();
    this.scheduleForm.patchValue({ jobApplicationId: app.id,
      id:0
     });
    this.loadSchedule(app.id);
    this.showModal = true;
  

  }

  loadSchedule(jobApplicationId: number)
  {
    this._companyService.getScheduleInterview(jobApplicationId).subscribe({
        next: (data: any[]) => {
          const interview = data[0];
          this.scheduleForm.patchValue({
            id:interview.id,
            interviewDate: interview.interviewDate,
            mode: interview.mode,
            locationOrLink: interview.locationOrLink
          })
          
        },
        error: () => {
          alert('Failed to load interview details.');
        }
      });    
  }

  closeModal() {
    this.showModal = false;
  }

  submitSchedule() {
    if (this.scheduleForm.invalid) return;
    
    this._companyService.scheduleInterview( this.scheduleForm.value)
      .subscribe({
        next: () => {
          alert('Interview scheduled!');
          this.closeModal();
        },
        error: () => {
          alert('Failed to schedule interview.');
        }
      });
  }
}
