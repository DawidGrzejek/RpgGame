import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { UserManagementService } from '../../services/user-management.service';
import { NotificationService } from '../../services/notification.service';
import {
  UserManagementDto,
  RoleManagementDto,
  CreateUserRequest,
  UpdateUserRequest,
  CreateRoleRequest,
  UpdateRoleRequest,
  UserLockoutRequest,
  ChangePasswordRequest,
  AssignRoleRequest,
  RemoveRoleRequest,
  UserSearchFilter,
  PagedResult
} from '../../models/user-management.model';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="user-management-container">
      <div class="header">
        <h1>User & Role Management</h1>
        <div class="header-actions">
          <button class="btn btn-primary" (click)="showCreateUserModal()">
            <span class="icon">👤</span> Add User
          </button>
          <button class="btn btn-secondary" (click)="showCreateRoleModal()">
            <span class="icon">🎭</span> Add Role
          </button>
        </div>
      </div>

      <!-- Tabs -->
      <div class="tabs">
        <button 
          class="tab-button" 
          [class.active]="activeTab === 'users'"
          (click)="setActiveTab('users')">
          Users ({{ usersTotal }})
        </button>
        <button 
          class="tab-button" 
          [class.active]="activeTab === 'roles'"
          (click)="setActiveTab('roles')">
          Roles ({{ roles.length }})
        </button>
      </div>

      <!-- Users Tab -->
      <div *ngIf="activeTab === 'users'" class="tab-content">
        <!-- Search and Filters -->
        <div class="filters">
          <div class="search-box">
            <input 
              type="text" 
              placeholder="Search users..." 
              [formControl]="searchControl"
              class="search-input">
            <span class="search-icon">🔍</span>
          </div>
          
          <div class="filter-group">
            <select [formControl]="roleFilterControl" class="filter-select">
              <option value="">All Roles</option>
              <option *ngFor="let role of roles" [value]="role.name">{{ role.name }}</option>
            </select>
            
            <select [formControl]="statusFilterControl" class="filter-select">
              <option value="">All Status</option>
              <option value="active">Active</option>
              <option value="locked">Locked Out</option>
              <option value="unconfirmed">Unconfirmed Email</option>
            </select>
          </div>
        </div>

        <!-- Users Table -->
        <div class="table-container" *ngIf="!isLoadingUsers">
          <table class="users-table">
            <thead>
              <tr>
                <th (click)="sortUsers('username')">
                  Username
                  <span class="sort-indicator" *ngIf="currentSort.field === 'username'">
                    {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
                  </span>
                </th>
                <th (click)="sortUsers('email')">
                  Email
                  <span class="sort-indicator" *ngIf="currentSort.field === 'email'">
                    {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
                  </span>
                </th>
                <th>Roles</th>
                <th>Status</th>
                <th (click)="sortUsers('createdAt')">
                  Created
                  <span class="sort-indicator" *ngIf="currentSort.field === 'createdAt'">
                    {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
                  </span>
                </th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let user of users" class="user-row">
                <td>
                  <div class="user-info">
                    <span class="username">{{ user.username }}</span>
                  </div>
                </td>
                <td>
                  <div class="email-info">
                    <span class="email">{{ user.email }}</span>
                    <span *ngIf="!user.emailConfirmed" class="unconfirmed-badge">Unconfirmed</span>
                  </div>
                </td>
                <td>
                  <div class="roles">
                    <span *ngFor="let role of user.roles" class="role-badge">{{ role }}</span>
                  </div>
                </td>
                <td>
                  <span class="status-badge" [class]="getUserStatusClass(user)">
                    {{ getUserStatus(user) }}
                  </span>
                </td>
                <td>{{ user.createdAt | date:'short' }}</td>
                <td>
                  <div class="actions">
                    <button class="action-btn edit" (click)="editUser(user)" title="Edit User">✏️</button>
                    <button class="action-btn roles" (click)="manageUserRoles(user)" title="Manage Roles">🎭</button>
                    <button *ngIf="isUserLocked(user)" class="action-btn unlock" (click)="unlockUser(user.id)" title="Unlock User">🔓</button>
                    <button *ngIf="!isUserLocked(user)" class="action-btn lock" (click)="lockUser(user)" title="Lock User">🔒</button>
                    <button class="action-btn reset" (click)="resetPassword(user)" title="Reset Password">🔑</button>
                    <button class="action-btn delete" (click)="deleteUser(user)" title="Delete User">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>

          <!-- Pagination -->
          <div class="pagination" *ngIf="totalPages > 1">
            <button 
              class="page-btn" 
              [disabled]="currentPage === 1"
              (click)="goToPage(currentPage - 1)">
              Previous
            </button>
            
            <span class="page-info">
              Page {{ currentPage }} of {{ totalPages }} ({{ usersTotal }} total)
            </span>
            
            <button 
              class="page-btn" 
              [disabled]="currentPage === totalPages"
              (click)="goToPage(currentPage + 1)">
              Next
            </button>
          </div>
        </div>

        <div *ngIf="isLoadingUsers" class="loading">Loading users...</div>
      </div>

      <!-- Roles Tab -->
      <div *ngIf="activeTab === 'roles'" class="tab-content">
        <div class="roles-grid">
          <div *ngFor="let role of roles" class="role-card">
            <div class="role-header">
              <h3>{{ role.name }}</h3>
              <div class="role-actions">
                <button class="action-btn edit" (click)="editRole(role)" title="Edit Role">✏️</button>
                <button class="action-btn delete" (click)="deleteRole(role)" title="Delete Role">🗑️</button>
              </div>
            </div>
            <p class="role-description">Role for managing system permissions</p>
            <div class="role-stats">
              <span class="users-count">{{ role.userCount }} users</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Create/Edit User Modal -->
      <div *ngIf="showUserModal" class="modal-overlay" (click)="closeUserModal()">
        <div class="modal" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h2>{{ editingUser ? 'Edit User' : 'Create User' }}</h2>
            <button class="close-btn" (click)="closeUserModal()">×</button>
          </div>
          
          <form [formGroup]="userForm" (ngSubmit)="saveUser()" class="modal-form">
            <div class="form-group">
              <label for="username">Username</label>
              <input type="text" id="username" formControlName="username" class="form-control">
              <div *ngIf="userForm.get('username')?.invalid && userForm.get('username')?.touched" class="error-message">
                Username is required (3-50 characters, letters, numbers, underscore only)
              </div>
            </div>

            <div class="form-group">
              <label for="email">Email</label>
              <input type="email" id="email" formControlName="email" class="form-control">
              <div *ngIf="userForm.get('email')?.invalid && userForm.get('email')?.touched" class="error-message">
                Valid email is required
              </div>
            </div>

            <div *ngIf="!editingUser" class="form-group">
              <label for="password">Password</label>
              <input type="password" id="password" formControlName="password" class="form-control">
              <div *ngIf="userForm.get('password')?.invalid && userForm.get('password')?.touched" class="error-message">
                Password is required (min 12 characters with uppercase, lowercase, and number)
              </div>
            </div>

            <div class="form-group">
              <label>Roles</label>
              <div class="roles-checkboxes">
                <label *ngFor="let role of roles" class="checkbox-label">
                  <input 
                    type="checkbox" 
                    [checked]="selectedRoles.includes(role.name)"
                    (change)="toggleRole(role.name, $event)">
                  <span class="checkbox-custom"></span>
                  {{ role.name }}
                </label>
              </div>
            </div>

            <div class="modal-actions">
              <button type="button" class="btn btn-secondary" (click)="closeUserModal()">Cancel</button>
              <button type="submit" class="btn btn-primary" [disabled]="userForm.invalid">
                {{ editingUser ? 'Update' : 'Create' }} User
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Create/Edit Role Modal -->
      <div *ngIf="showRoleModal" class="modal-overlay" (click)="closeRoleModal()">
        <div class="modal" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h2>{{ editingRole ? 'Edit Role' : 'Create Role' }}</h2>
            <button class="close-btn" (click)="closeRoleModal()">×</button>
          </div>
          
          <form [formGroup]="roleForm" (ngSubmit)="saveRole()" class="modal-form">
            <div class="form-group">
              <label for="roleName">Role Name</label>
              <input type="text" id="roleName" formControlName="name" class="form-control">
              <div *ngIf="roleForm.get('name')?.invalid && roleForm.get('name')?.touched" class="error-message">
                Role name is required
              </div>
            </div>


            <div class="modal-actions">
              <button type="button" class="btn btn-secondary" (click)="closeRoleModal()">Cancel</button>
              <button type="submit" class="btn btn-primary" [disabled]="roleForm.invalid">
                {{ editingRole ? 'Update' : 'Create' }} Role
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .user-management-container {
      padding: 2rem;
      max-width: 1400px;
      margin: 0 auto;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 2rem;
      padding-bottom: 1rem;
      border-bottom: 2px solid #e2e8f0;
    }

    .header h1 {
      color: #1a202c;
      margin: 0;
      font-size: 2rem;
      font-weight: 700;
    }

    .header-actions {
      display: flex;
      gap: 1rem;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-weight: 500;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      transition: all 0.2s;
      text-decoration: none;
    }

    .btn-primary {
      background-color: #3b82f6;
      color: white;
    }

    .btn-primary:hover {
      background-color: #2563eb;
    }

    .btn-secondary {
      background-color: #6b7280;
      color: white;
    }

    .btn-secondary:hover {
      background-color: #4b5563;
    }

    .tabs {
      display: flex;
      border-bottom: 1px solid #e2e8f0;
      margin-bottom: 2rem;
    }

    .tab-button {
      padding: 1rem 2rem;
      border: none;
      background: none;
      cursor: pointer;
      font-weight: 500;
      color: #6b7280;
      border-bottom: 2px solid transparent;
      transition: all 0.2s;
    }

    .tab-button.active {
      color: #3b82f6;
      border-bottom-color: #3b82f6;
    }

    .tab-button:hover {
      color: #3b82f6;
    }

    .filters {
      display: flex;
      gap: 1rem;
      margin-bottom: 2rem;
      align-items: center;
    }

    .search-box {
      position: relative;
      flex: 1;
      max-width: 400px;
    }

    .search-input {
      width: 100%;
      padding: 0.75rem 1rem 0.75rem 2.5rem;
      border: 1px solid #d1d5db;
      border-radius: 8px;
      font-size: 1rem;
    }

    .search-icon {
      position: absolute;
      left: 0.75rem;
      top: 50%;
      transform: translateY(-50%);
      color: #6b7280;
    }

    .filter-group {
      display: flex;
      gap: 1rem;
    }

    .filter-select {
      padding: 0.75rem;
      border: 1px solid #d1d5db;
      border-radius: 8px;
      background: white;
      min-width: 150px;
    }

    .table-container {
      background: white;
      border-radius: 12px;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
      overflow: hidden;
    }

    .users-table {
      width: 100%;
      border-collapse: collapse;
    }

    .users-table th {
      background-color: #f8fafc;
      padding: 1rem;
      text-align: left;
      font-weight: 600;
      color: #374151;
      cursor: pointer;
      user-select: none;
    }

    .users-table th:hover {
      background-color: #f1f5f9;
    }

    .sort-indicator {
      margin-left: 0.5rem;
      color: #3b82f6;
    }

    .users-table td {
      padding: 1rem;
      border-top: 1px solid #e5e7eb;
    }

    .user-row:hover {
      background-color: #f9fafb;
    }

    .user-info .username {
      font-weight: 500;
      color: #1f2937;
    }

    .email-info {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .unconfirmed-badge {
      font-size: 0.75rem;
      color: #dc2626;
      font-weight: 500;
    }

    .roles {
      display: flex;
      flex-wrap: wrap;
      gap: 0.25rem;
    }

    .role-badge {
      background-color: #e0e7ff;
      color: #3730a3;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.75rem;
      font-weight: 500;
    }

    .status-badge {
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.75rem;
      font-weight: 500;
    }

    .status-badge.active {
      background-color: #d1fae5;
      color: #065f46;
    }

    .status-badge.locked {
      background-color: #fee2e2;
      color: #991b1b;
    }

    .status-badge.unconfirmed {
      background-color: #fef3c7;
      color: #92400e;
    }

    .actions {
      display: flex;
      gap: 0.5rem;
    }

    .action-btn {
      padding: 0.25rem 0.5rem;
      border: none;
      background: none;
      cursor: pointer;
      border-radius: 4px;
      transition: background-color 0.2s;
    }

    .action-btn:hover {
      background-color: #f3f4f6;
    }

    .action-btn.delete:hover {
      background-color: #fee2e2;
    }

    .pagination {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 1rem;
      padding: 1rem;
      background-color: #f8fafc;
    }

    .page-btn {
      padding: 0.5rem 1rem;
      border: 1px solid #d1d5db;
      background: white;
      border-radius: 6px;
      cursor: pointer;
    }

    .page-btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .page-info {
      color: #6b7280;
      font-size: 0.875rem;
    }

    .roles-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 1.5rem;
    }

    .role-card {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    }

    .role-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }

    .role-header h3 {
      margin: 0;
      color: #1f2937;
    }

    .role-actions {
      display: flex;
      gap: 0.5rem;
    }

    .role-description {
      color: #6b7280;
      margin-bottom: 1rem;
    }

    .role-stats {
      font-size: 0.875rem;
      color: #6b7280;
    }

    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }

    .modal {
      background: white;
      border-radius: 12px;
      padding: 0;
      width: 90%;
      max-width: 500px;
      max-height: 90vh;
      overflow-y: auto;
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1.5rem;
      border-bottom: 1px solid #e5e7eb;
    }

    .modal-header h2 {
      margin: 0;
      color: #1f2937;
    }

    .close-btn {
      background: none;
      border: none;
      font-size: 1.5rem;
      cursor: pointer;
      color: #6b7280;
    }

    .modal-form {
      padding: 1.5rem;
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #374151;
    }

    .form-control {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #d1d5db;
      border-radius: 8px;
      font-size: 1rem;
      box-sizing: border-box;
    }

    .form-control:focus {
      outline: none;
      border-color: #3b82f6;
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
    }

    .error-message {
      color: #dc2626;
      font-size: 0.875rem;
      margin-top: 0.25rem;
    }

    .roles-checkboxes {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 0.5rem;
      margin-top: 0.5rem;
    }

    .modal-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      padding-top: 1rem;
      border-top: 1px solid #e5e7eb;
    }

    .loading {
      text-align: center;
      padding: 2rem;
      color: #6b7280;
    }

    .icon {
      font-size: 1rem;
    }
  `]
})
export class UserManagementComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  // Tab management
  activeTab: 'users' | 'roles' = 'users';

  // Users data
  users: UserManagementDto[] = [];
  usersTotal = 0;
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  isLoadingUsers = false;

  // Roles data
  roles: RoleManagementDto[] = [];

  // Search and filters
  searchControl!: FormControl;
  roleFilterControl!: FormControl;
  statusFilterControl!: FormControl;

  // Sorting
  currentSort = { field: 'username', direction: 'asc' as 'asc' | 'desc' };

  // Modals
  showUserModal = false;
  showRoleModal = false;
  editingUser: UserManagementDto | null = null;
  editingRole: RoleManagementDto | null = null;

  // Forms
  userForm: FormGroup;
  roleForm: FormGroup;
  selectedRoles: string[] = [];

  constructor(
    private fb: FormBuilder,
    private userManagementService: UserManagementService,
    private notificationService: NotificationService
  ) {
    this.userForm = this.createUserForm();
    this.roleForm = this.createRoleForm();
    
    // Initialize form controls
    this.searchControl = this.fb.control('');
    this.roleFilterControl = this.fb.control('');
    this.statusFilterControl = this.fb.control('');
  }

  ngOnInit(): void {
    console.log('🚀 UserManagementComponent.ngOnInit() called');
    this.loadRoles();
    this.loadUsers();
    this.setupSearchAndFilters();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private createUserForm(): FormGroup {
    return this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50), Validators.pattern(/^[a-zA-Z0-9_]+$/)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(12), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/)]]
    });
  }

  private createRoleForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]]
    });
  }

  private setupSearchAndFilters(): void {
    // Search debouncing
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.currentPage = 1;
        this.loadUsers();
      });

    // Filter changes
    this.roleFilterControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.currentPage = 1;
        this.loadUsers();
      });

    this.statusFilterControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.currentPage = 1;
        this.loadUsers();
      });
  }

  setActiveTab(tab: 'users' | 'roles'): void {
    this.activeTab = tab;
  }

  loadUsers(): void {
    this.isLoadingUsers = true;

    const filter: UserSearchFilter = {
      search: this.searchControl.value || undefined,
      role: this.roleFilterControl.value || undefined,
      page: this.currentPage,
      pageSize: this.pageSize,
      sortBy: this.currentSort.field,
      sortDirection: this.currentSort.direction
    };

    // Handle status filter
    const statusFilter = this.statusFilterControl.value;
    if (statusFilter === 'locked') {
      filter.isLockedOut = true;
    } else if (statusFilter === 'active') {
      filter.isLockedOut = false;
      filter.emailConfirmed = true;
    } else if (statusFilter === 'unconfirmed') {
      filter.emailConfirmed = false;
    }

    this.userManagementService.getUsers(filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.users = response.items;
          this.usersTotal = response.totalCount;
          this.totalPages = response.totalPages;
          this.isLoadingUsers = false;
        },
        error: (error) => {
          this.notificationService.showError('Error loading users: ' + error.message);
          this.isLoadingUsers = false;
        }
      });
  }

  loadRoles(): void {
    console.log('👥 UserManagementComponent.loadRoles() called');
    this.userManagementService.getRoles()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (roles) => {
          console.log('✅ Roles loaded successfully:', roles);
          this.roles = roles;
        },
        error: (error) => {
          console.error('❌ Error loading roles:', error);
          this.notificationService.showError('Error loading roles: ' + error.message);
        }
      });
  }

  sortUsers(field: string): void {
    if (this.currentSort.field === field) {
      this.currentSort.direction = this.currentSort.direction === 'asc' ? 'desc' : 'asc';
    } else {
      this.currentSort.field = field;
      this.currentSort.direction = 'asc';
    }
    this.loadUsers();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadUsers();
    }
  }

  getUserStatus(user: UserManagementDto): string {
    if (this.isUserLocked(user)) return 'Locked Out';
    if (!user.emailConfirmed) return 'Unconfirmed';
    if (!user.isActive) return 'Inactive';
    return 'Active';
  }

  getUserStatusClass(user: UserManagementDto): string {
    if (this.isUserLocked(user)) return 'locked';
    if (!user.emailConfirmed) return 'unconfirmed';
    if (!user.isActive) return 'locked';
    return 'active';
  }

  isUserLocked(user: UserManagementDto): boolean {
    return user.lockoutEnd != null && new Date(user.lockoutEnd) > new Date();
  }

  // User Modal Methods
  showCreateUserModal(): void {
    this.editingUser = null;
    this.selectedRoles = [];
    this.userForm.reset();
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(12), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/)]);
    this.showUserModal = true;
  }

  editUser(user: UserManagementDto): void {
    this.editingUser = user;
    this.selectedRoles = [...user.roles];
    this.userForm.patchValue({
      username: user.username,
      email: user.email
    });
    this.userForm.get('password')?.clearValidators();
    this.userForm.get('password')?.updateValueAndValidity();
    this.showUserModal = true;
  }

  closeUserModal(): void {
    this.showUserModal = false;
    this.editingUser = null;
    this.selectedRoles = [];
    this.userForm.reset();
  }

  toggleRole(roleName: string, event: any): void {
    if (event.target.checked) {
      if (!this.selectedRoles.includes(roleName)) {
        this.selectedRoles.push(roleName);
      }
    } else {
      this.selectedRoles = this.selectedRoles.filter(r => r !== roleName);
    }
  }

  saveUser(): void {
    if (this.userForm.valid) {
      const formValue = this.userForm.value;

      if (this.editingUser) {
        // Update user
        const request: UpdateUserRequest = {
          id: this.editingUser.id,
          username: formValue.username,
          email: formValue.email,
          emailConfirmed: this.editingUser.emailConfirmed,
          lockoutEnabled: this.editingUser.lockoutEnabled,
          lockoutEnd: this.editingUser.lockoutEnd,
          roles: this.selectedRoles,
          isActive: this.editingUser.isActive
        };

        this.userManagementService.updateUser(request)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.notificationService.showSuccess('User updated successfully');
              this.closeUserModal();
              this.loadUsers();
            },
            error: (error) => {
              this.notificationService.showError('Error updating user: ' + error.message);
            }
          });
      } else {
        // Create user
        const request: CreateUserRequest = {
          username: formValue.username,
          email: formValue.email,
          password: formValue.password,
          roles: this.selectedRoles,
          emailConfirmed: false,
          lockoutEnabled: true
        };

        this.userManagementService.createUser(request)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.notificationService.showSuccess('User created successfully');
              this.closeUserModal();
              this.loadUsers();
            },
            error: (error) => {
              this.notificationService.showError('Error creating user: ' + error.message);
            }
          });
      }
    }
  }

  // Role Modal Methods
  showCreateRoleModal(): void {
    this.editingRole = null;
    this.roleForm.reset();
    this.showRoleModal = true;
  }

  editRole(role: RoleManagementDto): void {
    this.editingRole = role;
    this.roleForm.patchValue({
      name: role.name
    });
    this.showRoleModal = true;
  }

  closeRoleModal(): void {
    this.showRoleModal = false;
    this.editingRole = null;
    this.roleForm.reset();
  }

  saveRole(): void {
    if (this.roleForm.valid) {
      const formValue = this.roleForm.value;

      if (this.editingRole) {
        // Update role
        const request: UpdateRoleRequest = {
          id: this.editingRole.id,
          name: formValue.name
        };

        this.userManagementService.updateRole(request)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.notificationService.showSuccess('Role updated successfully');
              this.closeRoleModal();
              this.loadRoles();
            },
            error: (error) => {
              this.notificationService.showError('Error updating role: ' + error.message);
            }
          });
      } else {
        // Create role
        const request: CreateRoleRequest = {
          name: formValue.name
        };

        this.userManagementService.createRole(request)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.notificationService.showSuccess('Role created successfully');
              this.closeRoleModal();
              this.loadRoles();
            },
            error: (error) => {
              this.notificationService.showError('Error creating role: ' + error.message);
            }
          });
      }
    }
  }

  // User Actions
  manageUserRoles(user: UserManagementDto): void {
    // Open user edit modal with focus on roles
    this.editUser(user);
  }

  lockUser(user: UserManagementDto): void {
    if (confirm(`Are you sure you want to lock out ${user.username}?`)) {
      const request: UserLockoutRequest = {
        userId: user.id,
        lockoutEnd: new Date(Date.now() + (30 * 24 * 60 * 60 * 1000)), // 30 days
        reason: 'Locked via admin panel'
      };

      this.userManagementService.lockUser(request)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.notificationService.showSuccess('User locked out successfully');
            this.loadUsers();
          },
          error: (error) => {
            this.notificationService.showError('Error locking user: ' + error.message);
          }
        });
    }
  }

  unlockUser(userId: string): void {
    this.userManagementService.unlockUser(userId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('User unlocked successfully');
          this.loadUsers();
        },
        error: (error) => {
          this.notificationService.showError('Error unlocking user: ' + error.message);
        }
      });
  }

  resetPassword(user: UserManagementDto): void {
    const newPassword = prompt(`Enter new password for ${user.username}:`);
    if (newPassword) {
      const request: ChangePasswordRequest = {
        userId: user.id,
        newPassword: newPassword
      };

      this.userManagementService.changePassword(request)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.notificationService.showSuccess('Password reset successfully');
          },
          error: (error) => {
            this.notificationService.showError('Error resetting password: ' + error.message);
          }
        });
    }
  }

  deleteUser(user: UserManagementDto): void {
    if (confirm(`Are you sure you want to delete ${user.username}? This action cannot be undone.`)) {
      this.userManagementService.deleteUser(user.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.notificationService.showSuccess('User deleted successfully');
            this.loadUsers();
          },
          error: (error) => {
            this.notificationService.showError('Error deleting user: ' + error.message);
          }
        });
    }
  }

  deleteRole(role: RoleManagementDto): void {
    if (confirm(`Are you sure you want to delete the role "${role.name}"? This action cannot be undone.`)) {
      this.userManagementService.deleteRole(role.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.notificationService.showSuccess('Role deleted successfully');
            this.loadRoles();
          },
          error: (error) => {
            this.notificationService.showError('Error deleting role: ' + error.message);
          }
        });
    }
  }
}