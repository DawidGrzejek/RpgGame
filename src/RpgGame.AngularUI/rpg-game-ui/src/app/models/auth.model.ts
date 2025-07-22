export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  isSuccess: boolean;
  accessToken: string;
  refreshToken: string;
  expiresAt: Date;
  user: UserInfo;
  errors: string[];
}

export interface UserInfo {
  id: string;
  username: string;
  email: string;
  roles: string[];
  createdAt?: string;
  lastLoginAt?: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
  confirmNewPassword: string;
}

// Auth state management
export interface AuthState {
  isAuthenticated: boolean;
  isLoading: boolean;
  user: UserInfo | null;
  accessToken: string | null;
  refreshToken: string | null;
  expiresAt: Date | null;
  error: string | null;
}

export const initialAuthState: AuthState = {
  isAuthenticated: false,
  isLoading: false,
  user: null,
  accessToken: null,
  refreshToken: null,
  expiresAt: null,
  error: null
};


// Role-based permissions
export enum UserRole {
  PLAYER = 'Player',
  ADMIN = 'Admin',
  MODERATOR = 'Moderator',
  GUEST = 'Guest'
}

export interface Permission {
  name: string;
  description: string;
  roles: UserRole[];
}

// Common auth errors
export enum AuthError {
  INVALID_CREDENTIALS = 'Invalid email or password',
  USER_NOT_FOUND = 'User not found',
  EMAIL_TAKEN = 'Email already exists',
  USERNAME_TAKEN = 'Username already exists',
  TOKEN_EXPIRED = 'Session expired',
  UNAUTHORIZED = 'Unauthorized access',
  NETWORK_ERROR = 'Network connection failed',
  VALIDATION_ERROR = 'Validation failed'
}
