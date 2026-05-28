import { AuthUser } from '../../core/models/auth.models';
import { AuthActions } from './auth.actions';
import { createReducer, on } from '@ngrx/store';

export interface AuthState {
  user: AuthUser | null;
  token: string | null;
  loading: boolean;
  error: string | null;
  isAuthenticated: boolean;
}

export const initialAuthState: AuthState = {
  user: null,
  token: null,
  loading: false,
  error: null,
  isAuthenticated: false,
};

export const authReducer = createReducer(
  initialAuthState,
  on(AuthActions.signIn, AuthActions.signUp, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(AuthActions.authSuccess, (state, { user, token }) => ({
    ...state,
    user,
    token,
    loading: false,
    error: null,
    isAuthenticated: true,
  })),
  on(AuthActions.authFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
    isAuthenticated: false,
  })),
  on(AuthActions.logout, AuthActions.unauthorized, () => ({
    ...initialAuthState,
  }))
);
