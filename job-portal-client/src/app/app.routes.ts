import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';


// Lazy import or direct import—here we use dynamic for smaller initial bundle
export const routes: Routes = [
  // Student login
  { path: 'student/login', loadComponent: () => import('./features/student/login.component').then(m => m.StudentLoginComponent) },

  // Company login
  { path: 'company/login', loadComponent: () => import('./features/company/login.component').then(m => m.CompanyLoginComponent) },

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
  }, /* 
  {
    path: 'student/profile',
    canActivate: [AuthGuard, RoleGuard], data: { roles: ['Student'] },
    loadComponent: () => import('./features/student/profile.component').then(m => m.StudentProfileComponent)  
  },
  {
    path: 'student/resume',
    canActivate: [AuthGuard, RoleGuard], data: { roles: ['Student'] },
    loadComponent: () => import('./features/student/resume.component').then(m => m.StudentResumeComponent)  
  },
{
    path: 'student/messages',
    canActivate: [AuthGuard, RoleGuard], data: { roles: ['Student'] },
    loadComponent: () => import('./features/student/messages.component').then(m => m.StudentMessagesComponent)
  },
  {
    path: 'student/jobs',
    canActivate: [AuthGuard, RoleGuard], data: { roles: ['Student'] },
    loadComponent: () => import('./features/student/jobs.component').then(m => m.StudentJobsComponent)
  },*/
  {
    path: 'student/register',
    loadComponent: () => import('./features/student/registration.component').then(m => m.RegistrationComponent)
  },
  {
    path: 'company/dashboard',
    canActivate: [AuthGuard, RoleGuard], data: { roles: ['Company'] },
    loadComponent: () => import('./features/company/dashboard.component').then(m => m.CompanyDashboardComponent)
  },

  // Default route → student login (adjust if you prefer a landing page)
  { path: '', pathMatch: 'full', redirectTo: 'student/login' },
  { path: '**', redirectTo: 'student/login' }
];
