import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, take, switchMap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private authService: AuthService) {
    console.log('🎯 AuthInterceptor constructor called');
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    console.log('🔍 AuthInterceptor.intercept() called for URL:', request.url);
    
    // Add auth token to requests
    const authRequest = this.addTokenToRequest(request);

    return next.handle(authRequest).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('🚨 HTTP Error in AuthInterceptor:', {
          url: request.url,
          status: error.status,
          message: error.message
        });
        
        // Handle 401 errors (unauthorized)
        if (error.status === 401 && !authRequest.url.includes('/auth/')) {
          console.log('🔄 Attempting to handle 401 error for:', request.url);
          return this.handle401Error(authRequest, next);
        }

        return throwError(() => error);
      })
    );
  }

  /**
   * Add authentication token to request headers
   */
  private addTokenToRequest(request: HttpRequest<any>): HttpRequest<any> {
    const accessToken = this.authService.getAccessToken();
    
    console.log('Auth Interceptor Debug:', {
      url: request.url,
      hasToken: !!accessToken,
      tokenPreview: accessToken ? `${accessToken.substring(0, 20)}...` : 'null',
      isAuthUrl: request.url.includes('/auth/')
    });

    if (accessToken && !request.url.includes('/auth/login') && !request.url.includes('/auth/register')) {
      const authorizedRequest = request.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`
        }
      });
      
      console.log('Added Authorization header:', authorizedRequest.headers.get('Authorization')?.substring(0, 30) + '...');
      return authorizedRequest;
    }

    console.log('No token added to request');
    return request;
  }

  /**
   * Handle 401 unauthorized errors by attempting token refresh
   */
  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response) => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(response.accessToken);

          // Retry the original request with new token
          const newAuthRequest = this.addTokenToRequest(request);
          return next.handle(newAuthRequest);
        }),
        catchError((error) => {
          this.isRefreshing = false;

          // Refresh failed, logout user
          this.authService.logout();
          return throwError(() => error);
        })
      );
    } else {
      // Wait for refresh to complete, then retry request
      return this.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(() => {
          const newAuthRequest = this.addTokenToRequest(request);
          return next.handle(newAuthRequest);
        })
      );
    }
  }
}
