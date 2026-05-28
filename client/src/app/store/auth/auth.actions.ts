import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { AuthUser, SignInRequest, SignUpRequest } from '../../core/models/auth.models';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    Init: emptyProps(),
    'Sign In': props<{ request: SignInRequest }>(),
    'Sign Up': props<{ request: SignUpRequest }>(),
    'Auth Success': props<{ user: AuthUser; token: string }>(),
    'Auth Failure': props<{ error: string }>(),
    Logout: emptyProps(),
    Unauthorized: emptyProps(),
  },
});
