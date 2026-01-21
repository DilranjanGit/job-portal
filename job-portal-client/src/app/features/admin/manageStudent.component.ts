import { Component, OnInit } from '@angular/core';
import { StudentService } from '../../core/services/student.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manage-Student',
  imports:[CommonModule],
  templateUrl: './manageStudent.component.html',
  styleUrls: ['./manageCompany.component.scss']
})
export class ManageStudentComponent implements OnInit {
  Students: any[] = [];
  loadingId: number | null = null;
  error: string = '';

  constructor(private StudentService: StudentService) {}

  ngOnInit() {
    this.fetchStudents();
  }

  fetchStudents() {
    this.StudentService.getAllStudents().subscribe({
      next: (data) => {
        console.log(data);
        this.Students = data; },
      error: (err) => { this.error = 'Failed to load Students.'; }
    });
  }

  toggleStatus(Student: any) {
    this.loadingId = Student.id;
    this.StudentService.updateStudentStatus(Student.id, !Student.isActive).subscribe({
      next: () => {
        Student.isActive = !Student.isActive;
        this.loadingId = null;
      },
      error: () => {
        this.error = 'Failed to update status.';
        this.loadingId = null;
      }
    });
  }
}