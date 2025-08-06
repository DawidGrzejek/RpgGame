// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError, timer } from 'rxjs';
import { map, catchError, tap, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  UserInfo,
  AuthState,
  initialAuthState,
  RefreshTokenRequest,
  ChangePasswordRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  AuthError
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:7153/api/v1/auth';
  private readonly storageKeys = {
    accessToken: 'rpg_access_token',
    refreshToken: 'rpg_refresh_token',
    user: 'rpg_user',
    expiresAt: 'rpg_expires_at'
  };

  // Auth state management
  private authStateSubject = new BehaviorSubject<AuthState>(initialAuthState);
  public authState$ = this.authStateSubject.asObservable();

  // Convenience observables
  public isAuthenticated$ = this.authState$.pipe(map(state => state.isAuthenticated));
  public currentUser$ = this.authState$.pipe(map(state => state.user));
  public isLoading$ = this.authState$.pipe(map(state => state.isLoading));
  public error$ = this.authState$.pipe(map(state => state.error));

  private refreshTimer?: any;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.initializeAuthState();
    this.startTokenRefreshTimer();
  }

  /**
   * Initialize authentication state from local storage
   */
  private initializeAuthState(): void {
    try {
      const accessToken = localStorage.getItem(this.storageKeys.accessToken);
      const refreshToken = localStorage.getItem(this.storageKeys.refreshToken);
      const userJson = localStorage.getItem(this.storageKeys.user);
      const expiresAtStr = localStorage.getItem(this.storageKeys.expiresAt);

      if (accessToken && refreshToken && userJson && expiresAtStr) {
        const user: UserInfo = JSON.parse(userJson);
        const expiresAt = new Date(expiresAtStr);

        // Check if token is still valid
        if (expiresAt > new Date()) {
          this.updateAuthState({
            isAuthenticated: true,
            isLoading: false,
            user,
            accessToken,
            refreshToken,
            expiresAt,
            error: null
          });
        } else {
          // Token expired, try to refresh
          this.refreshTokenSilently();
        }
      }
    } catch (error) {
      console.error('Error initializing auth state:', error);
      this.logout();
    }
  }

  /**
   * User login
   */
  login(credentials: LoginRequest): Observable<AuthResponse> {
    this.setLoading(true);

    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(response => {
          if (response.isSuccess) {
            this.handleAuthSuccess(response);
          } else {
            this.handleAuthError(response.errors.join(', '));
          }
        }),
        catchError(error => this.handleHttpError(error)),
        tap(() => this.setLoading(false))
      );
  }

  /**
   * User registration
   */
  register(userData: RegisterRequest): Observable<AuthResponse> {
    this.setLoading(true);

    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, userData)
      .pipe(
        tap(response => {
          if (response.isSuccess) {
            this.handleAuthSuccess(response);
          } else {
            this.handleAuthError(response.errors.join(', '));
          }
        }),
        catchError(error => this.handleHttpError(error)),
        tap(() => this.setLoading(false))
      );
  }

  /**
   * Refresh access token
   */
  refreshToken(): Observable<AuthResponse> {
    const currentState = this.authStateSubject.value;

    if (!currentState.refreshToken) {
      return throwError(() => new Error('No refresh token available'));
    }

    const refreshRequest: RefreshTokenRequest = {
      refreshToken: currentState.refreshToken
    };

    return this.http.post<AuthResponse>(`${this.apiUrl}/refresh`, refreshRequest)
      .pipe(
        tap(response => {
          if (response.isSuccess) {
            this.handleAuthSuccess(response);
          } else {
            this.logout();
          }
        }),
        catchError(error => {
          this.logout();
          return this.handleHttpError(error);
        })
      );
  }

  /**
   * Silent token refresh (for automatic renewal)
   */
  private refreshTokenSilently(): void {
    this.refreshToken().subscribe({
      next: () => {
        console.log('Token refreshed successfully');
      },
      error: (error) => {
        console.error('Silent token refresh failed:', error);
        this.logout();
      }
    });
  }

  /**
   * User logout
   */
  logout(): void {
    // Clear refresh timer
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
    }

    // Clear local storage
    Object.values(this.storageKeys).forEach(key => {
      localStorage.removeItem(key);
    });

    // Reset auth state
    this.updateAuthState(initialAuthState);

    // Navigate to login page
    this.router.navigate(['/auth/login']);
  }

  /**
   * Change password
   */
  changePassword(passwordData: ChangePasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/change-password`, passwordData)
      .pipe(
        catchError(error => this.handleHttpError(error))
      );
  }

  /**
   * Forgot password
   */
  forgotPassword(request: ForgotPasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, request)
      .pipe(
        catchError(error => this.handleHttpError(error))
      );
  }

  /**
   * Reset password
   */
  resetPassword(request: ResetPasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, request)
      .pipe(
        catchError(error => this.handleHttpError(error))
      );
  }

  /**
   * Get current user info
   */
  getCurrentUser(): UserInfo | null {
    return this.authStateSubject.value.user;
  }

  /**
   * Get access token
   */
  getAccessToken(): string | null {
    return this.authStateSubject.value.accessToken;
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return this.authStateSubject.value.isAuthenticated;
  }

  /**
   * Check if user has specific role
   */
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.roles.includes(role) ?? false;
  }

  /**
   * Check if user has any of the specified roles
   */
  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUser();
    return roles.some(role => user?.roles.includes(role)) ?? false;
  }

  /**
   * Check if user is admin
   */
  isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  /**
   * Check if user is moderator or admin
   */
  isModerator(): boolean {
    return this.hasAnyRole(['Admin', 'Moderator']);
  }

  /**
   * Handle successful authentication
   */
  private handleAuthSuccess(response: AuthResponse): void {
    const expiresAt = new Date(response.expiresAt);

    // Store in localStorage
    localStorage.setItem(this.storageKeys.accessToken, response.accessToken);
    localStorage.setItem(this.storageKeys.refreshToken, response.refreshToken);
    localStorage.setItem(this.storageKeys.user, JSON.stringify(response.user));
    localStorage.setItem(this.storageKeys.expiresAt, expiresAt.toISOString());

    // Update auth state
    this.updateAuthState({
      isAuthenticated: true,
      isLoading: false,
      user: response.user,
      accessToken: response.accessToken,
      refreshToken: response.refreshToken,
      expiresAt,
      error: null
    });

    // Start token refresh timer
    this.startTokenRefreshTimer();
  }

  /**
   * Handle authentication error
   */
  private handleAuthError(error: string): void {
    this.updateAuthState({
      ...this.authStateSubject.value,
      isLoading: false,
      error
    });
  }

  /**
   * Handle HTTP errors
   */
  private handleHttpError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = AuthError.NETWORK_ERROR;

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message as AuthError;
    } else {
      // Server-side error
      switch (error.status) {
        case 401:
          errorMessage = AuthError.UNAUTHORIZED;
          this.logout();
          break;
        case 400:
          // Check if it's an AuthResponse with validation errors
          if (error.error && error.error.errors && Array.isArray(error.error.errors)) {
            errorMessage = error.error.errors.join(', ');
          } else {
            // Map known error messages to AuthError enum, otherwise use VALIDATION_ERROR
            errorMessage = (Object.values(AuthError) as string[]).includes(error.error?.message)
              ? error.error.message as AuthError
              : AuthError.VALIDATION_ERROR;
          }
          break;
        case 404:
          errorMessage = AuthError.USER_NOT_FOUND;
          break;
        default:
          errorMessage = error.error?.message || AuthError.NETWORK_ERROR;
      }
    }

    this.handleAuthError(errorMessage);
    return throwError(() => new Error(errorMessage));
  }

  /**
   * Update auth state
   */
  private updateAuthState(newState: AuthState): void {
    this.authStateSubject.next(newState);
  }

  /**
   * Set loading state
   */
  private setLoading(isLoading: boolean): void {
    this.updateAuthState({
      ...this.authStateSubject.value,
      isLoading
    });
  }

  /**
   * Start automatic token refresh timer
   */
  private startTokenRefreshTimer(): void {
    const currentState = this.authStateSubject.value;

    if (currentState.expiresAt && currentState.isAuthenticated) {
      const expiresAt = currentState.expiresAt.getTime();
      const now = Date.now();
      const refreshTime = expiresAt - (5 * 60 * 1000); // Refresh 5 minutes before expiry
      const timeUntilRefresh = refreshTime - now;

      if (timeUntilRefresh > 0) {
        this.refreshTimer = setTimeout(() => {
          this.refreshTokenSilently();
        }, timeUntilRefresh);
      } else {
        // Token expires soon, refresh immediately
        this.refreshTokenSilently();
      }
    }
  }

  /**
   * Clear auth error
   */
  clearError(): void {
    this.updateAuthState({
      ...this.authStateSubject.value,
      error: null
    });
  }
}
