import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../../core/services/company.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manage-company',
  imports:[CommonModule],
  templateUrl: './manageCompany.component.html',
  styleUrls: ['./manageCompany.component.scss']
})
export class ManageCompanyComponent implements OnInit {
  companies: any[] = [];
  loadingId: number | null = null;
  error: string = '';

  constructor(private companyService: CompanyService) {}

  ngOnInit() {
    this.fetchCompanies();
  }

  fetchCompanies() {
    this.companyService.getAllCompanies().subscribe({
      next: (data) => {
        console.log(data);
        this.companies = data; },
      error: (err) => { this.error = 'Failed to load companies.'; }
    });
  }

  toggleStatus(company: any) {
    this.loadingId = company.id;
    this.companyService.updateCompanyStatus(company.id, !company.isActive).subscribe({
      next: () => {
        company.isActive = !company.isActive;
        this.loadingId = null;
      },
      error: () => {
        this.error = 'Failed to update status.';
        this.loadingId = null;
      }
    });
  }
}