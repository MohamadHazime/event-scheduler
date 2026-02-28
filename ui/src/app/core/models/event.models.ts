export interface CreateEventRequest {
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  location: string;
  category?: string;
}

export interface UpdateEventRequest {
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  location: string;
  category?: string;
}

export interface EventResponse {
  id: string;
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  location: string;
  category: string;
  createdById: string;
  createdByName: string;
  createdAt: string;
  updatedAt: string;
  attendees: AttendeeResponse[];
}

export interface AttendeeResponse {
  userId: string;
  fullName: string;
  email: string;
  status: string;
}

export interface EventListQuery {
  title?: string;
  location?: string;
  category?: string;
  fromDate?: string;
  toDate?: string;
  page?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}