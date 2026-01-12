import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authHeaderInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const user = authService.currentUser;

  if (!user) {
    return next(req);
  }

  const updatedRequest = req.clone({
    setHeaders: {
      'x-user-id': user.id
    }
  });

  return next(updatedRequest);
};
