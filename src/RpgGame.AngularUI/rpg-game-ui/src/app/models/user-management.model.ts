// User Management Models
export interface UserManagementDto {
  id: string;
  username: string;
  email: string;
  emailConfirmed: boolean;
  lockoutEnabled: boolean;
  lockoutEnd?: Date;
  accessFailedCount: number;
  roles: string[];
  createdAt: Date;
  lastLoginAt?: Date;
  isActive: boolean;
}

export interface RoleManagementDto {
  id: string;
  name: string;
  normalizedName: string;
  userCount: number;
  createdAt: Date;
}

export interface CreateUserRequest {
  username: string;
  email: string;
  password: string;
  roles: string[];
  emailConfirmed: boolean;
  lockoutEnabled: boolean;
}

export interface UpdateUserRequest {
  id: string;
  username: string;
  email: string;
  emailConfirmed: boolean;
  lockoutEnabled: boolean;
  lockoutEnd?: Date;
  roles: string[];
  isActive: boolean;
}

export interface CreateRoleRequest {
  name: string;
}

export interface UpdateRoleRequest {
  id: string;
  name: string;
}

export interface UserRoleRequest {
  userId: string;
  roleName: string;
}

export interface UserLockoutRequest {
  userId: string;
  lockoutEnd?: Date;
  reason: string;
}

export interface ChangePasswordRequest {
  userId: string;
  newPassword: string;
}

export interface AssignRoleRequest {
  userId: string;
  roleName: string;
}

export interface RemoveRoleRequest {
  userId: string;
  roleName: string;
}

export interface ApiResponse<T> {
  isSuccess: boolean;
  data?: T;
  errors: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

export interface UserSearchFilter {
  search?: string;
  role?: string;
  isLockedOut?: boolean;
  emailConfirmed?: boolean;
  page: number;
  pageSize: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}