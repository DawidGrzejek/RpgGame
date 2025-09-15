import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  UserManagementDto,
  RoleManagementDto,
  CreateUserRequest,
  UpdateUserRequest,
  CreateRoleRequest,
  UpdateRoleRequest,
  AssignRoleRequest,
  RemoveRoleRequest,
  UserLockoutRequest,
  ChangePasswordRequest,
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
  getUsers(filter: UserSearchFilter): Observable<PagedResult<UserManagementDto>> {
    let params = new HttpParams()
      .set('page', filter.page.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.search) {
      params = params.set('searchTerm', filter.search);
    }
    if (filter.role) {
      params = params.set('roleFilter', filter.role);
    }

    return this.http.get<PagedResult<UserManagementDto>>(`${this.apiUrl}/usermanagement`, { params });
  }

  getUserById(id: string): Observable<UserManagementDto> {
    return this.http.get<UserManagementDto>(`${this.apiUrl}/usermanagement/${id}`);
  }

  createUser(request: CreateUserRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/usermanagement`, request);
  }

  updateUser(request: UpdateUserRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/usermanagement/${request.id}`, request);
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/usermanagement/${id}`);
  }

  lockUser(request: UserLockoutRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/usermanagement/${request.userId}/lock`, request);
  }

  unlockUser(userId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/usermanagement/${userId}/unlock`, {});
  }

  changePassword(request: ChangePasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/usermanagement/${request.userId}/change-password`, request);
  }

  assignRole(request: AssignRoleRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/usermanagement/${request.userId}/roles`, request);
  }

  removeRole(request: RemoveRoleRequest): Observable<any> {
    return this.http.delete(`${this.apiUrl}/usermanagement/${request.userId}/roles`, { body: request });
  }

  // Role Operations
  getRoles(): Observable<RoleManagementDto[]> {
    return this.http.get<RoleManagementDto[]>(`${this.apiUrl}/rolemanagement`);
  }

  getRoleById(id: string): Observable<RoleManagementDto> {
    return this.http.get<RoleManagementDto>(`${this.apiUrl}/rolemanagement/${id}`);
  }

  createRole(request: CreateRoleRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/rolemanagement`, request);
  }

  updateRole(request: UpdateRoleRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/rolemanagement/${request.id}`, request);
  }

  deleteRole(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/rolemanagement/${id}`);
  }
}