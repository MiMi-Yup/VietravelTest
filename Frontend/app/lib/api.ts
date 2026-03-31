// Centralized API client with auth token management

const API_BASE = '/api';

function getStoredToken(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem('token');
}

function clearAuth() {
  if (typeof window === 'undefined') return;
  localStorage.removeItem('token');
  localStorage.removeItem('username');
}

export function isLoggedIn(): boolean {
  return !!getStoredToken();
}

export function getUsername(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem('username');
}

export function logout() {
  clearAuth();
  window.location.href = '/login';
}

export async function login(username: string, password: string) {
  const res = await fetch(`${API_BASE}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  });

  const data = await res.json();
  if (!res.ok) throw new Error(data.message || 'Đăng nhập thất bại.');

  localStorage.setItem('token', data.token);
  localStorage.setItem('username', data.username);
  return data;
}

async function apiRequest<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getStoredToken();

  // If no token, redirect to login
  if (!token && typeof window !== 'undefined') {
    window.location.href = '/login';
    throw new Error('Chưa đăng nhập.');
  }

  const res = await fetch(`${API_BASE}${endpoint}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
    cache: 'no-store',
  });

  // If unauthorized, redirect to login
  if (res.status === 401) {
    clearAuth();
    if (typeof window !== 'undefined') {
      window.location.href = '/login';
    }
    throw new Error('Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.');
  }

  if (!res.ok) {
    const errorData = await res.json().catch(() => null);
    throw new Error(errorData?.message || `Lỗi hệ thống: ${res.status}`);
  }

  // For 204 No Content
  if (res.status === 204) return {} as T;

  return res.json();
}

// ===== Tour APIs =====
import type {
  Tour,
  BookingRequest,
  PagedResponse,
  CreateTourDto,
  UpdateTourDto,
  CreateBookingRequestDto,
} from './types';

export async function getTours(pageNumber = 1, pageSize = 10) {
  return apiRequest<PagedResponse<Tour>>(
    `/tours?pageNumber=${pageNumber}&pageSize=${pageSize}`
  );
}

export async function getTourById(id: string) {
  return apiRequest<Tour>(`/tours/${id}`);
}

export async function createTour(data: CreateTourDto) {
  return apiRequest<Tour>('/tours', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export async function updateTour(id: string, data: UpdateTourDto) {
  return apiRequest<Tour>(`/tours/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}

export async function updateTourStatus(id: string, isActive: boolean) {
  return apiRequest<void>(`/tours/${id}/status`, {
    method: 'PATCH',
    body: JSON.stringify({ isActive }),
  });
}

export async function deleteTour(id: string) {
  return apiRequest<void>(`/tours/${id}`, { method: 'DELETE' });
}

// ===== Booking Request APIs =====
export async function getRequests(pageNumber = 1, pageSize = 10) {
  return apiRequest<PagedResponse<BookingRequest>>(
    `/requests?pageNumber=${pageNumber}&pageSize=${pageSize}`
  );
}

export async function getRequestById(id: string) {
  return apiRequest<BookingRequest>(`/requests/${id}`);
}

export async function createRequest(data: CreateBookingRequestDto) {
  return apiRequest<BookingRequest>('/requests', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export async function updateRequest(
  id: string,
  data: CreateBookingRequestDto
) {
  return apiRequest<BookingRequest>(`/requests/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}

export async function approveRequest(id: string) {
  return apiRequest<BookingRequest>(`/requests/${id}/approve`, {
    method: 'POST',
  });
}

export async function deleteRequest(id: string) {
  return apiRequest<void>(`/requests/${id}`, { method: 'DELETE' });
}
