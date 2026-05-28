import { authReducer, initialAuthState } from './auth.reducer';
import { AuthActions } from './auth.actions';

describe('authReducer', () => {
  it('should set loading on sign in', () => {
    const state = authReducer(
      initialAuthState,
      AuthActions.signIn({ request: { email: 'a@b.com', password: 'secret' } })
    );

    expect(state.loading).toBe(true);
    expect(state.error).toBeNull();
  });

  it('should store user and token on auth success', () => {
    const state = authReducer(
      initialAuthState,
      AuthActions.authSuccess({
        user: { userId: '1', email: 'a@b.com' },
        token: 'token',
      })
    );

    expect(state.isAuthenticated).toBe(true);
    expect(state.token).toBe('token');
    expect(state.user?.email).toBe('a@b.com');
  });

  it('should reset state on logout', () => {
    const authenticated = authReducer(
      initialAuthState,
      AuthActions.authSuccess({
        user: { userId: '1', email: 'a@b.com' },
        token: 'token',
      })
    );

    const state = authReducer(authenticated, AuthActions.logout());
    expect(state).toEqual(initialAuthState);
  });
});
