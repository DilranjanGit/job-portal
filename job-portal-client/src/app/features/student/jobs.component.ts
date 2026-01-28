
// src/app/features/student/jobs.component.ts
export interface Job {
    
  id: number;
  title: string;
  description: string;
  companyName: string;
  location: string;
  education: string;
  skills: string;   // array of skill strings
  applied: boolean;   // true if student already applied
  scheduled: boolean; // true if inteview already scheduled
}


import { Component, OnInit} from '@angular/core';
import { StudentService } from '../../core/services/student.service';
import { CompanyService } from '../../core/services/company.service';
import { ActivatedRoute } from '@angular/router';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-jobs',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './jobs.component.html',
  styleUrls: ['./jobs.component.scss']
})



export class JobsComponent implements OnInit {
  jobs: Job[] = [];
  applying: Set<number> = new Set();
  error: string | null = null;
  email: string = "";
  schedule: boolean = false;
  showModal = false;

  constructor(private studentService: StudentService, private companyService: CompanyService, private route: ActivatedRoute) {
    
    this.route.queryParams.subscribe(params => {
       this.email = params['email'] || '';
     });


  }
   
  ngOnInit(): void {
    this.loadJobs();
  }

  loadJobs(): void {
    this.studentService.getAllJobs(this.email).subscribe({
      next: (data) => {
      this.jobs = (data || []).map((dto: any) => ({
        id: dto.id ?? dto.jobId ?? 0,
        title: dto.title ?? dto.jobTitle ?? '',
        companyName:dto.companyName ?? '',
        location: dto.location ?? '',
        description: dto.description??'',
        education: dto.education ?? '',
        skills: dto.skills ?? '',     
        applied: dto.applied ?? false,
        scheduled: dto.scheduled ?? false
        
    }));

      },
      error: () => {
        this.error = 'Failed to load jobs. Please try again.';
      }
    });
  }

  isApplying(jobId: number): boolean {
    return this.applying.has(jobId);
  }

  
  apply(jobId: number): void {
    this.error = null;

    if (this.applying.has(jobId)) return;

    this.applying.add(jobId);

    // optimistic UI update
    const job = this.jobs.find(j => j.id === jobId);
    if (job) job.applied = true;
   const payload = {studentEmail: this.email,jobId: jobId.toString()}
    this.studentService.ApplyJob(payload).subscribe({
      next: () => {
        this.applying.delete(jobId);
      },
      error: () => {
        // revert if failed
        if (job) job.applied = false;

        this.applying.delete(jobId);
        this.error = 'Failed to apply. Please try again.';
      }
    });
  }

  trackByJobId(_: number, job: Job) {
    return job.id;
  }
 interview={
  scheduledAt:'',
  location:'',
  mode:'',
  status:''
 };
  showSchedule(jobId: number) {
    this.showModal=true;
     this.companyService.getScheduleInterview(jobId).subscribe({
      next: (data: any[])=>{
        const inter = data[0];
         // this.scheduleForm.patchValue({
          this.interview.scheduledAt= inter.interviewDate,
          this.interview.mode= inter.mode,
          this.interview.location= inter.locationOrLink,
          this.interview.status=inter.status
           
      },
      error: err=> {
        this.error="Failed to fetch interview details", err;
      },
     });
  }
  closeModal(){
    this.showModal=false;
  }
}
