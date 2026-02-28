import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateEventRequest,
  UpdateEventRequest,
  EventResponse,
  EventListQuery,
  PagedResult,
  AttendeeResponse
} from '../models/event.models';

@Injectable({ providedIn: 'root' })
export class EventService {
  private readonly apiUrl = `${environment.apiUrl}/events`;

  constructor(private http: HttpClient) {}

  getEvents(query: EventListQuery): Observable<PagedResult<EventResponse>> {
    let params = new HttpParams();
    if (query.title) params = params.set('title', query.title);
    if (query.location) params = params.set('location', query.location);
    if (query.category) params = params.set('category', query.category);
    if (query.fromDate) params = params.set('fromDate', query.fromDate);
    if (query.toDate) params = params.set('toDate', query.toDate);
    if (query.page) params = params.set('page', query.page.toString());
    if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());

    return this.http.get<PagedResult<EventResponse>>(this.apiUrl, { params });
  }

  getEventById(id: string): Observable<EventResponse> {
    return this.http.get<EventResponse>(`${this.apiUrl}/${id}`);
  }

  createEvent(request: CreateEventRequest): Observable<EventResponse> {
    return this.http.post<EventResponse>(this.apiUrl, request);
  }

  updateEvent(id: string, request: UpdateEventRequest): Observable<EventResponse> {
    return this.http.put<EventResponse>(`${this.apiUrl}/${id}`, request);
  }

  deleteEvent(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }

  updateStatus(eventId: string, status: string): Observable<AttendeeResponse> {
    return this.http.put<AttendeeResponse>(`${this.apiUrl}/${eventId}/status`, { status });
  }

  getAttendees(eventId: string): Observable<AttendeeResponse[]> {
    return this.http.get<AttendeeResponse[]>(`${this.apiUrl}/${eventId}/attendees`);
  }
}