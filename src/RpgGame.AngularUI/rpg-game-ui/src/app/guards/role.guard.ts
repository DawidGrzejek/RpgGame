import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    const requiredRoles = route.data['roles'] as string[];
    const requireAll = route.data['requireAll'] as boolean || false;

    return this.authService.currentUser$.pipe(
      take(1),
      map(user => {
        if (!user) {
          this.router.navigate(['/auth/login']);
          return false;
        }

        const hasRequiredRole = requireAll
          ? requiredRoles.every(role => user.roles.includes(role))
          : requiredRoles.some(role => user.roles.includes(role));

        if (!hasRequiredRole) {
          this.notificationService.showError('You do not have permission to access this page');
          this.router.navigate(['/']);
          return false;
        }

        return true;
      })
    );
  }
}
