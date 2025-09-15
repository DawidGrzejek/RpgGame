import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { inject } from '@angular/core';

import { routes } from './app.routes';
import { AuthService } from './services/auth.service';
import { NotificationService } from './services/notification.service';

// Convert class-based interceptors to functional interceptors
export const authInterceptor = (req: any, next: any) => {
  const authService = inject(AuthService);
  
  console.log('🔍 Functional AuthInterceptor called for URL:', req.url);
  
  // Get access token
  const accessToken = authService.getAccessToken();
  
  console.log('🔑 Token check in functional interceptor:', {
    hasToken: !!accessToken,
    tokenPreview: accessToken ? `${accessToken.substring(0, 20)}...` : 'null',
    url: req.url
  });

  // Add auth token to requests (skip auth endpoints)
  if (accessToken && !req.url.includes('/auth/login') && !req.url.includes('/auth/register')) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`
      }
    });
    console.log('✅ Added Authorization header to request');
    return next(authReq);
  }

  console.log('⚠️ No token added to request');
  return next(req);
};

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor])
    ),
    // Explicitly provide services
    AuthService,
    NotificationService,
    importProvidersFrom(BrowserAnimationsModule)
  ]
};
