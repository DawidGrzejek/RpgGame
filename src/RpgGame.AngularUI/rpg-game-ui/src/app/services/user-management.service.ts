import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  User,
  Role,
  CreateUserRequest,
  UpdateUserRequest,
  CreateRoleRequest,
  UpdateRoleRequest,
  UserRoleRequest,
  UserLockoutRequest,
  PasswordResetRequest,
  ApiResponse,
  PagedResult,
  UserSearchFilter
} from '../models/user-management.model';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {
  private readonly apiUrl = 'https://localhost:7153/api/v1';

  constructor(private http: HttpClient) { }

  // User Operations
  getUsers(filter: UserSearchFilter): Observable<ApiResponse<PagedResult<User>>> {
    let params = new HttpParams()
      .set('page', filter.page.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.search) {
      params = params.set('search', filter.search);
    }
    if (filter.role) {
      params = params.set('role', filter.role);
    }
    if (filter.isLockedOut !== undefined) {
      params = params.set('isLockedOut', filter.isLockedOut.toString());
    }
    if (filter.emailConfirmed !== undefined) {
      params = params.set('emailConfirmed', filter.emailConfirmed.toString());
    }
    if (filter.sortBy) {
      params = params.set('sortBy', filter.sortBy);
    }
    if (filter.sortDirection) {
      params = params.set('sortDirection', filter.sortDirection);
    }

    return this.http.get<ApiResponse<PagedResult<User>>>(`${this.apiUrl}/admin/users`, { params });
  }

  getUserById(id: string): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(`${this.apiUrl}/admin/users/${id}`);
  }

  createUser(request: CreateUserRequest): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(`${this.apiUrl}/admin/users`, request);
  }

  updateUser(request: UpdateUserRequest): Observable<ApiResponse<User>> {
    return this.http.put<ApiResponse<User>>(`${this.apiUrl}/admin/users/${request.id}`, request);
  }

  deleteUser(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/admin/users/${id}`);
  }

  lockoutUser(request: UserLockoutRequest): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/admin/users/${request.userId}/lockout`, request);
  }

  unlockUser(userId: string): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/admin/users/${userId}/unlock`, {});
  }

  resetUserPassword(request: PasswordResetRequest): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/admin/users/${request.userId}/reset-password`, request);
  }

  confirmUserEmail(userId: string): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/admin/users/${userId}/confirm-email`, {});
  }

  // Role Operations
  getRoles(): Observable<ApiResponse<Role[]>> {
    return this.http.get<ApiResponse<Role[]>>(`${this.apiUrl}/admin/roles`);
  }

  getRoleById(id: string): Observable<ApiResponse<Role>> {
    return this.http.get<ApiResponse<Role>>(`${this.apiUrl}/admin/roles/${id}`);
  }

  createRole(request: CreateRoleRequest): Observable<ApiResponse<Role>> {
    return this.http.post<ApiResponse<Role>>(`${this.apiUrl}/admin/roles`, request);
  }

  updateRole(request: UpdateRoleRequest): Observable<ApiResponse<Role>> {
    return this.http.put<ApiResponse<Role>>(`${this.apiUrl}/admin/roles/${request.id}`, request);
  }

  deleteRole(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/admin/roles/${id}`);
  }

  // User-Role Operations
  addUserToRole(request: UserRoleRequest): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/admin/users/${request.userId}/roles`, request);
  }

  removeUserFromRole(request: UserRoleRequest): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/admin/users/${request.userId}/roles/${request.roleName}`);
  }

  getUserRoles(userId: string): Observable<ApiResponse<string[]>> {
    return this.http.get<ApiResponse<string[]>>(`${this.apiUrl}/admin/users/${userId}/roles`);
  }
}