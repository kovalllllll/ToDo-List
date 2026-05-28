import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { catchError, throwError } from 'rxjs';
import { AuthActions } from '../../store/auth/auth.actions';
import { selectAuthToken } from '../../store/auth/auth.selectors';

const PUBLIC_URLS = ['/users/sign-in', '/users/sign-up'];

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const store = inject(Store);
  const token = store.selectSignal(selectAuthToken)();

  const isPublic = PUBLIC_URLS.some((url) => req.url.includes(url));
  const authReq =
    token && !isPublic
      ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : req;

  return next(authReq).pipe(
    catchError((error) => {
      if (error.status === 401 && !isPublic) {
        store.dispatch(AuthActions.unauthorized());
      }
      return throwError(() => error.error ?? error);
    })
  );
};
