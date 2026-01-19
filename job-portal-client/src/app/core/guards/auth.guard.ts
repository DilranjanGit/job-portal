
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const AuthGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  
  if (!auth.isLoggedIn()) {
    router.navigate(['/login/login']); // redirect to login
    return false;
  }
  return true;
};
``
