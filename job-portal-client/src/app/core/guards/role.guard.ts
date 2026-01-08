
import { CanActivateFn, ActivatedRouteSnapshot, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const RoleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const allowedRoles = route.data['roles'] as string[];
  const userRole = auth.getRole();
  if (!userRole || !allowedRoles.includes(userRole)) {
    router.navigate(['/student/login']); // redirect if role mismatch
    return false;
  }
  return true;
};
