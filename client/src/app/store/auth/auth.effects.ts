import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, filter, map, of, switchMap, tap } from 'rxjs';
import {
  AUTH_TOKEN_KEY,
  AUTH_USER_KEY,
} from '../../core/models/auth.models';
import { getErrorMessage } from '../../core/models/api-error.models';
import { AuthService } from '../../core/services/auth.service';
import { AuthActions } from './auth.actions';

@Injectable()
export class AuthEffects {
  private readonly actions$ = inject(Actions);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  init$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.init),
      switchMap(() => {
        const token = localStorage.getItem(AUTH_TOKEN_KEY);
        const userRaw = localStorage.getItem(AUTH_USER_KEY);

        if (token && userRaw) {
          return of(
            AuthActions.authSuccess({
              user: JSON.parse(userRaw),
              token,
            })
          );
        }

        return of({ type: '[Auth] Init Skipped' });
      }),
      filter((action) => action.type !== '[Auth] Init Skipped')
    )
  );

  signIn$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.signIn),
      switchMap(({ request }) =>
        this.authService.signIn(request).pipe(
          map((response) =>
            AuthActions.authSuccess({
              user: { userId: response.userId, email: response.email },
              token: response.accessToken,
            })
          ),
          catchError((error) =>
            of(AuthActions.authFailure({ error: getErrorMessage(error, 'Invalid credentials') }))
          )
        )
      )
    )
  );

  signUp$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.signUp),
      switchMap(({ request }) =>
        this.authService.signUp(request).pipe(
          map((response) =>
            AuthActions.authSuccess({
              user: { userId: response.userId, email: response.email },
              token: response.accessToken,
            })
          ),
          catchError((error) =>
            of(AuthActions.authFailure({ error: getErrorMessage(error, 'Registration failed') }))
          )
        )
      )
    )
  );

  authSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.authSuccess),
        tap(({ user, token }) => {
          localStorage.setItem(AUTH_TOKEN_KEY, token);
          localStorage.setItem(AUTH_USER_KEY, JSON.stringify(user));
          this.router.navigate(['/projects']);
        })
      ),
    { dispatch: false }
  );

  logout$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.logout, AuthActions.unauthorized),
        tap(() => {
          localStorage.removeItem(AUTH_TOKEN_KEY);
          localStorage.removeItem(AUTH_USER_KEY);
          this.router.navigate(['/auth/login']);
        })
      ),
    { dispatch: false }
  );
}
