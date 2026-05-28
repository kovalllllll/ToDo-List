export interface AuthResponse {
  userId: string;
  email: string;
  accessToken: string;
}

export interface SignInRequest {
  email: string;
  password: string;
}

export interface SignUpRequest {
  email: string;
  userName: string;
  password: string;
}

export interface AuthUser {
  userId: string;
  email: string;
}

export const AUTH_TOKEN_KEY = 'todo_access_token';
export const AUTH_USER_KEY = 'todo_auth_user';
