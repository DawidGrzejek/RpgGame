// User Management Models
export interface User {
  id: string;
  username: string;
  email: string;
  emailConfirmed: boolean;
  roles: string[];
  isLockedOut: boolean;
  lockoutEnd?: Date;
  createdAt: Date;
  lastLoginAt?: Date;
}

export interface Role {
  id: string;
  name: string;
  description?: string;
  usersCount: number;
}

export interface CreateUserRequest {
  username: string;
  email: string;
  password: string;
  roles: string[];
}

export interface UpdateUserRequest {
  id: string;
  username: string;
  email: string;
  roles: string[];
}

export interface CreateRoleRequest {
  name: string;
  description?: string;
}

export interface UpdateRoleRequest {
  id: string;
  name: string;
  description?: string;
}

export interface UserRoleRequest {
  userId: string;
  roleName: string;
}

export interface UserLockoutRequest {
  userId: string;
  lockoutEnd?: Date; // If null, unlocks user
}

export interface PasswordResetRequest {
  userId: string;
  newPassword: string;
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