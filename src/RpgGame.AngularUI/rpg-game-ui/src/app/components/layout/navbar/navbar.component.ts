import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthService } from '../../../services/auth.service';
import { UserInfo } from '../../../models/auth.model';
import { HasRoleDirective } from '../../../directives/has-role.directive';
import { IsAuthenticatedDirective } from '../../../directives/is-authenticated.directive';
import { IsGuestDirective } from '../../../directives/is-guest.directive';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    HasRoleDirective,
    IsAuthenticatedDirective,
    IsGuestDirective
  ],
  template: `
    <nav class="navbar">
      <div class="nav-container">
        <!-- Logo -->
        <div class="nav-brand">
          <a routerLink="/" class="brand-link">
            <span class="brand-icon">⚔️</span>
            <span class="brand-text">RPG Game</span>
          </a>
        </div>

        <!-- Navigation Links -->
        <div class="nav-links" [class.mobile-open]="isMobileMenuOpen">
          <!-- Authenticated User Links -->
          <ng-container *appIsAuthenticated>
            <a routerLink="/characters" routerLinkActive="active" class="nav-link">
              <span class="nav-icon">🏰</span>
              Characters
            </a>

            <a routerLink="/characters/new" routerLinkActive="active" class="nav-link">
              <span class="nav-icon">➕</span>
              Create Character
            </a>

            <!-- Admin Links -->
            <div *appHasRole="'Admin'" class="nav-dropdown">
              <button class="nav-link dropdown-toggle" (click)="toggleAdminMenu($event)">
                <span class="nav-icon">⚙️</span>
                Admin
                <span class="dropdown-arrow">▼</span>
              </button>
              <div class="dropdown-menu" [class.show]="showAdminMenu">
                <a routerLink="/admin/entities" class="dropdown-item">
                  <span class="nav-icon">📊</span>
                  Manage Entities
                </a>
                <a routerLink="/admin/users" class="dropdown-item">
                  <span class="nav-icon">👥</span>
                  User Management
                </a>
                <a routerLink="/admin/analytics" class="dropdown-item">
                  <span class="nav-icon">📈</span>
                  Analytics
                </a>
              </div>
            </div>

            <!-- Moderation Links - Available to both Admin and Moderator users -->
            <ng-container *appHasRole="['Admin', 'Moderator']">
              <a routerLink="/moderation" routerLinkActive="active" class="nav-link">
                <span class="nav-icon">🛡️</span>
                Moderation
              </a>
            </ng-container>
          </ng-container>

          <!-- Guest Links -->
          <ng-container *appIsGuest>
            <a routerLink="/about" routerLinkActive="active" class="nav-link">
              About
            </a>
            <a routerLink="/features" routerLinkActive="active" class="nav-link">
              Features
            </a>
          </ng-container>
        </div>

        <!-- User Menu -->
        <div class="nav-user">
          <!-- Authenticated User Menu -->
          <div *appIsAuthenticated class="user-menu">
            <div class="user-dropdown">
              <button class="user-toggle" (click)="toggleUserMenu($event)">
                <div class="user-avatar">
                  {{ getUserInitials() }}
                </div>
                <div class="user-info">
                  <span class="user-name">{{ currentUser?.username }}</span>
                  <span class="user-role">{{ getUserPrimaryRole() }}</span>
                </div>
                <span class="dropdown-arrow">▼</span>
              </button>

              <div class="dropdown-menu user-dropdown-menu" [class.show]="showUserMenu">
                <div class="dropdown-header">
                  <div class="user-avatar-large">
                    {{ getUserInitials() }}
                  </div>
                  <div class="user-details">
                    <div class="user-name">{{ currentUser?.username }}</div>
                    <div class="user-email">{{ currentUser?.email }}</div>
                  </div>
                </div>

                <div class="dropdown-divider"></div>

                <a routerLink="/profile" class="dropdown-item" (click)="closeUserMenu()">
                  <span class="nav-icon">👤</span>
                  Profile
                </a>

                <a routerLink="/settings" class="dropdown-item" (click)="closeUserMenu()">
                  <span class="nav-icon">⚙️</span>
                  Settings
                </a>

                <a routerLink="/achievements" class="dropdown-item" (click)="closeUserMenu()">
                  <span class="nav-icon">🏆</span>
                  Achievements
                </a>

                <div class="dropdown-divider"></div>

                <button class="dropdown-item logout-item" (click)="logout()">
                  <span class="nav-icon">🚪</span>
                  Sign Out
                </button>
              </div>
            </div>
          </div>

          <!-- Guest Actions -->
          <div *appIsGuest class="guest-actions">
            <a routerLink="/auth/login" class="btn btn-outline">
              Sign In
            </a>
            <a routerLink="/auth/register" class="btn btn-primary">
              Sign Up
            </a>
          </div>

          <!-- Mobile Menu Toggle -->
          <button class="mobile-toggle" (click)="toggleMobileMenu()">
            <span class="hamburger" [class.open]="isMobileMenuOpen">
              <span></span>
              <span></span>
              <span></span>
            </span>
          </button>
        </div>
      </div>
    </nav>

    <!-- Mobile Overlay -->
    <div
      class="mobile-overlay"
      [class.show]="isMobileMenuOpen"
      (click)="closeMobileMenu()">
    </div>
  `,
  styles: [`
    .navbar {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      position: sticky;
      top: 0;
      z-index: 1000;
    }

    .nav-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 20px;
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 64px;
    }

    /* Brand */
    .nav-brand {
      flex-shrink: 0;
    }

    .brand-link {
      display: flex;
      align-items: center;
      gap: 8px;
      text-decoration: none;
      color: white;
      font-weight: 700;
      font-size: 20px;
    }

    .brand-icon {
      font-size: 24px;
    }

    .brand-text {
      font-family: 'Cinzel', serif;
    }

    /* Navigation Links */
    .nav-links {
      display: flex;
      align-items: center;
      gap: 24px;
      flex: 1;
      justify-content: center;
    }

    .nav-link {
      display: flex;
      align-items: center;
      gap: 6px;
      color: rgba(255, 255, 255, 0.9);
      text-decoration: none;
      padding: 8px 16px;
      border-radius: 8px;
      transition: all 0.2s;
      background: none;
      border: none;
      cursor: pointer;
      font-size: 14px;
      font-weight: 500;
    }

    .nav-link:hover {
      background: rgba(255, 255, 255, 0.1);
      color: white;
    }

    .nav-link.active {
      background: rgba(255, 255, 255, 0.2);
      color: white;
    }

    .nav-icon {
      font-size: 16px;
    }

    /* Dropdowns */
    .nav-dropdown {
      position: relative;
    }

    .dropdown-toggle {
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .dropdown-arrow {
      font-size: 12px;
      transition: transform 0.2s;
    }

    .dropdown-toggle:hover .dropdown-arrow {
      transform: rotate(180deg);
    }

    .dropdown-menu {
      position: absolute;
      top: 100%;
      left: 0;
      background: white;
      border-radius: 8px;
      box-shadow: 0 10px 25px rgba(0,0,0,0.15);
      min-width: 200px;
      opacity: 0;
      visibility: hidden;
      transform: translateY(-10px);
      transition: all 0.2s;
      z-index: 1001;
    }

    .dropdown-menu.show {
      opacity: 1;
      visibility: visible;
      transform: translateY(0);
    }

    .dropdown-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 16px;
      color: #374151;
      text-decoration: none;
      border: none;
      background: none;
      width: 100%;
      text-align: left;
      cursor: pointer;
      transition: background-color 0.2s;
    }

    .dropdown-item:hover {
      background: #f3f4f6;
    }

    .dropdown-item:first-child {
      border-radius: 8px 8px 0 0;
    }

    .dropdown-item:last-child {
      border-radius: 0 0 8px 8px;
    }

    .dropdown-header {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 16px;
      border-bottom: 1px solid #e5e7eb;
    }

    .dropdown-divider {
      height: 1px;
      background: #e5e7eb;
      margin: 8px 0;
    }

    .logout-item {
      color: #dc2626;
    }

    .logout-item:hover {
      background: #fee2e2;
    }

    /* User Menu */
    .nav-user {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .user-menu {
      position: relative;
    }

    .user-toggle {
      display: flex;
      align-items: center;
      gap: 12px;
      background: rgba(255, 255, 255, 0.1);
      border: none;
      border-radius: 8px;
      padding: 8px 12px;
      color: white;
      cursor: pointer;
      transition: background-color 0.2s;
    }

    .user-toggle:hover {
      background: rgba(255, 255, 255, 0.2);
    }

    .user-avatar, .user-avatar-large {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
      font-weight: 600;
      font-size: 14px;
    }

    .user-avatar-large {
      width: 48px;
      height: 48px;
      font-size: 18px;
    }

    .user-info {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
    }

    .user-name {
      font-weight: 500;
      font-size: 14px;
    }

    .user-role {
      font-size: 12px;
      opacity: 0.8;
    }

    .user-email {
      font-size: 12px;
      color: #6b7280;
    }

    .user-dropdown-menu {
      right: 0;
      left: auto;
      min-width: 250px;
    }

    .user-details {
      display: flex;
      flex-direction: column;
    }

    /* Guest Actions */
    .guest-actions {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .btn {
      padding: 8px 16px;
      border-radius: 6px;
      text-decoration: none;
      font-weight: 500;
      font-size: 14px;
      transition: all 0.2s;
      border: 1px solid transparent;
      display: inline-flex;
      align-items: center;
      justify-content: center;
    }

    .btn-outline {
      color: white;
      border-color: rgba(255, 255, 255, 0.3);
    }

    .btn-outline:hover {
      background: rgba(255, 255, 255, 0.1);
    }

    .btn-primary {
      background: white;
      color: #667eea;
    }

    .btn-primary:hover {
      background: #f8fafc;
    }

    /* Mobile */
    .mobile-toggle {
      display: none;
      background: none;
      border: none;
      padding: 8px;
      cursor: pointer;
    }

    .hamburger {
      width: 24px;
      height: 18px;
      position: relative;
      transform: rotate(0deg);
      transition: 0.3s ease-in-out;
      cursor: pointer;
    }

    .hamburger span {
      display: block;
      position: absolute;
      height: 2px;
      width: 100%;
      background: white;
      border-radius: 1px;
      opacity: 1;
      left: 0;
      transform: rotate(0deg);
      transition: 0.25s ease-in-out;
    }

    .hamburger span:nth-child(1) {
      top: 0px;
    }

    .hamburger span:nth-child(2) {
      top: 8px;
    }

    .hamburger span:nth-child(3) {
      top: 16px;
    }

    .hamburger.open span:nth-child(1) {
      top: 8px;
      transform: rotate(135deg);
    }

    .hamburger.open span:nth-child(2) {
      opacity: 0;
      left: -60px;
    }

    .hamburger.open span:nth-child(3) {
      top: 8px;
      transform: rotate(-135deg);
    }

    .mobile-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(0, 0, 0, 0.5);
      opacity: 0;
      visibility: hidden;
      transition: all 0.3s;
      z-index: 999;
    }

    .mobile-overlay.show {
      opacity: 1;
      visibility: visible;
    }

    @media (max-width: 768px) {
      .mobile-toggle {
        display: block;
      }

      .nav-links {
        position: fixed;
        top: 64px;
        left: -100%;
        width: 280px;
        height: calc(100vh - 64px);
        background: white;
        flex-direction: column;
        align-items: stretch;
        padding: 20px;
        gap: 0;
        transition: left 0.3s ease;
        z-index: 1001;
        box-shadow: 2px 0 10px rgba(0,0,0,0.1);
      }

      .nav-links.mobile-open {
        left: 0;
      }

      .nav-link {
        color: #374151;
        padding: 16px 0;
        border-bottom: 1px solid #e5e7eb;
        border-radius: 0;
      }

      .nav-link:hover {
        background: transparent;
        color: #667eea;
      }

      .nav-link.active {
        background: transparent;
        color: #667eea;
      }

      .guest-actions {
        flex-direction: column;
        width: 100%;
        gap: 8px;
        margin-top: 20px;
      }

      .btn {
        width: 100%;
        justify-content: center;
      }

      .user-info {
        display: none;
      }
    }
  `]
})
export class NavbarComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private boundDocumentClick!: (event: Event) => void;

  currentUser: UserInfo | null = null;
  isAuthenticated = false;
  showUserMenu = false;
  showAdminMenu = false;
  isMobileMenuOpen = false;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // Subscribe to auth state
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.currentUser = user;
      });

    this.authService.isAuthenticated$
      .pipe(takeUntil(this.destroy$))
      .subscribe(isAuth => {
        this.isAuthenticated = isAuth;
      });

    // Close menus when clicking outside
    this.boundDocumentClick = this.handleDocumentClick.bind(this);
    document.addEventListener('click', this.boundDocumentClick);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    document.removeEventListener('click', this.boundDocumentClick);
  }

  getUserInitials(): string {
    if (!this.currentUser?.username) return '?';
    return this.currentUser.username.charAt(0).toUpperCase();
  }

  getUserPrimaryRole(): string {
    if (!this.currentUser?.roles || this.currentUser.roles.length === 0) {
      return 'Player';
    }

    // Prioritize roles
    if (this.currentUser.roles.includes('Admin')) return 'Admin';
    if (this.currentUser.roles.includes('Moderator')) return 'Moderator';
    return this.currentUser.roles[0];
  }

  toggleUserMenu(event: Event): void {
    event.stopPropagation();
    this.showUserMenu = !this.showUserMenu;
    this.showAdminMenu = false;
  }

  toggleAdminMenu(event: Event): void {
    event.stopPropagation();
    this.showAdminMenu = !this.showAdminMenu;
    this.showUserMenu = false;
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  closeUserMenu(): void {
    this.showUserMenu = false;
  }

  closeAdminMenu(): void {
    this.showAdminMenu = false;
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
  }

  logout(): void {
    this.authService.logout();
    this.closeUserMenu();
  }

  private handleDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;

    // Close user menu if clicking outside
    if (!target.closest('.user-dropdown')) {
      this.showUserMenu = false;
    }

    // Close admin menu if clicking outside
    if (!target.closest('.nav-dropdown')) {
      this.showAdminMenu = false;
    }
  }
}
