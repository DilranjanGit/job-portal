import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';


// Lazy import or direct import—here we use dynamic for smaller initial bundle
export const routes: Routes = [
  // Login route
  { path: 'login/login', loadComponent: () => import('./features/login/login.component').then(m => m.LoginComponent) },

   // Example protected routes (use your real components)
  {
    path: 'student/dashboard',
    canActivate: [AuthGuard, RoleGuard], data: { roles: ['Student'] },
    canActivateChild: [AuthGuard, RoleGuard],
    loadComponent: () => import('./features/student/dashboard.component').then(m => m.StudentDashboardComponent),
    children:[
      {
        path: 'profile',
        loadComponent: () => import('./features/student/profile.component').then(m => m.StudentProfileComponent)
      },
      {
        path: 'resume',
   
        loadComponent: () => import('./features/student/resume.component').then(m=>m.StudentResumeComponent)
      }
    ]
  },
  {
    path: 'student/register',
    loadComponent: () => import('./features/student/registration.component').then(m => m.RegistrationComponent)
  },
  {
    path: 'company/register',
    loadComponent: () => import('./features/company/registration.component').then(m => m.CompanyRegistrationComponent)
  },
  {
    path: 'company/dashboard',
    canActivate: [AuthGuard, RoleGuard], data: { roles: ['Company'] },
    loadComponent: () => import('./features/company/dashboard.component').then(m => m.CompanyDashboardComponent)
  },
  {
    path: 'admin/dashboard',
    canActivate: [AuthGuard, RoleGuard], data: { roles: ['Admin','Student','Company'] },
    loadComponent: () => import('./features/admin/dashboard.component').then(m => m.AdminDashboardComponent)  
  },
  {
    path: 'dashboard',
    canActivate: [AuthGuard, RoleGuard], data: { roles: ['Admin','Student','Company'] },
    loadComponent: () => import('./features/dashboard.component').then(m => m.AdminDashboardComponent) ,
     children:[
      {
        path: 'profile',
        loadComponent: () => import('./features/student/profile.component').then(m => m.StudentProfileComponent)
      },
      {
        path: 'resume',
   
        loadComponent: () => import('./features/student/resume.component').then(m=>m.StudentResumeComponent)
      },
      {
        path: 'postJobs',
   
        loadComponent: () => import('./features/company/postJobs.component').then(m=>m.PostJobsComponent)
      }
    ] 
  },

  // Default route → student login (adjust if you prefer a landing page)
  { path: '', pathMatch: 'full', redirectTo: 'login/login' },
  { path: '**', redirectTo: 'login/login' }
];
