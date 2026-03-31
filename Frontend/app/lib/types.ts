// TypeScript interfaces matching the backend API responses

export interface Tour {
  id: string;
  name: string;
  description?: string;
  price: number;
  city: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface RequestDetail {
  id: string;
  serviceType: string;
  serviceName: string;
  supplier: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
  note?: string;
}

export interface BookingRequest {
  id: string;
  tourName: string;
  departureDate: string;
  personInCharge: string;
  tourType: 'FIT' | 'GIT' | 'MICE';
  guestCount: number;
  status: 'Received' | 'PendingApproval' | 'Approved';
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
  details: RequestDetail[];
}

export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// Request DTOs
export interface CreateTourDto {
  name: string;
  description?: string;
  price: number;
  city: string;
}

export interface UpdateTourDto {
  name: string;
  description?: string;
  price: number;
  city: string;
}

export interface CreateRequestDetailDto {
  serviceType: string;
  serviceName: string;
  supplier: string;
  quantity: number;
  unitPrice: number;
  note?: string;
}

export interface CreateBookingRequestDto {
  tourName: string;
  departureDate: string;
  personInCharge: string;
  tourType: 'FIT' | 'GIT' | 'MICE';
  guestCount: number;
  details: CreateRequestDetailDto[];
}
