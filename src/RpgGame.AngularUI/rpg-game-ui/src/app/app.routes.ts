import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';
import { RoleGuard } from './guards/role.guard';
import { GuestGuard } from './guards/guest.guard';

// Lazy-loaded components
import { HomeComponent } from './components/home/home.component';
import { CharacterListComponent } from './components/character-list/character-list.component';
import { CharacterDetailComponent } from './components/character-detail/character-detail.component';
import { CharacterCreationComponent } from './components/character-creation/character-creation.component';
import { EntityHierarchyComponent } from './components/entity-hierarchy/entity-hierarchy.component';
import { EnemyCreationComponent } from './components/enemy-creation/enemy-creation.component';
import { ItemCreationComponent } from './components/item-creation/item-creation.component';

// Auth components
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';

// Admin components
import { UserManagementComponent } from './components/admin/user-management.component';

export const routes: Routes = [
  // Public routes
  { path: '', component: HomeComponent },

  // Auth routes (guest only)
  {
    path: 'auth',
    canActivate: [GuestGuard],
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      // {
      //   path: 'forgot-password',
      //   loadComponent: () => import('./components/auth/forgot-password/forgot-password.component').then(c => c.ForgotPasswordComponent)
      // },
      // {
      //   path: 'reset-password',
      //   loadComponent: () => import('./components/auth/reset-password/reset-password.component').then(c => c.ResetPasswordComponent)
      // },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  },

  // Protected routes (require authentication)
  {
    path: 'characters',
    canActivate: [AuthGuard],
    children: [
      { path: '', component: CharacterListComponent },
      { path: 'new', component: CharacterCreationComponent },
      { path: ':id', component: CharacterDetailComponent }
    ]
  },

  // User profile routes
  // {
  //   path: 'profile',
  //   canActivate: [AuthGuard],
  //   loadComponent: () => import('./components/user/profile/profile.component').then(c => c.ProfileComponent)
  // },
  // {
  //   path: 'settings',
  //   canActivate: [AuthGuard],
  //   loadComponent: () => import('./components/user/settings/settings.component').then(c => c.SettingsComponent)
  // },
  // {
  //   path: 'achievements',
  //   canActivate: [AuthGuard],
  //   loadComponent: () => import('./components/user/achievements/achievements.component').then(c => c.AchievementsComponent)
  // },

  // Moderator routes
  // {
  //   path: 'moderation',
  //   canActivate: [AuthGuard, RoleGuard],
  //   data: { roles: ['Admin', 'Moderator'] },
  //   loadComponent: () => import('./components/moderation/moderation.component').then(c => c.ModerationComponent)
  // },

  // Admin routes
  {
    path: 'admin',
    canActivate: [AuthGuard, AdminGuard],
    children: [
      { path: 'entities', component: EntityHierarchyComponent },
      { path: 'enemies/new', component: EnemyCreationComponent },
      { path: 'enemies/:id/edit', component: EnemyCreationComponent },
      { path: 'items/new', component: ItemCreationComponent },
      { path: 'items/:id/edit', component: ItemCreationComponent },
      { path: 'users', component: UserManagementComponent },
      // {
      //   path: 'analytics',
      //   loadComponent: () => import('./components/admin/analytics/analytics.component').then(c => c.AnalyticsComponent)
      // },
      { path: '', redirectTo: 'entities', pathMatch: 'full' }
    ]
  },

  // Error pages
  // {
  //   path: 'unauthorized',
  //   loadComponent: () => import('./components/error/unauthorized/unauthorized.component').then(c => c.UnauthorizedComponent)
  // },
  // {
  //   path: 'not-found',
  //   loadComponent: () => import('./components/error/not-found/not-found.component').then(c => c.NotFoundComponent)
  // },

  // Wildcard route - must be last
  { path: '**', redirectTo: 'not-found' }
];
