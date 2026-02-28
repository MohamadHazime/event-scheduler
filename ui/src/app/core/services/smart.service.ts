import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ConflictCheckRequest,
  ConflictResult,
  CategorizationRequest,
  CategorizationResult,
  SlotSuggestion,
  AnalyticsResponse
} from '../models/smart.models';

@Injectable({ providedIn: 'root' })
export class SmartService {
  private readonly apiUrl = `${environment.apiUrl}/smart`;

  constructor(private http: HttpClient) {}

  checkConflicts(request: ConflictCheckRequest): Observable<ConflictResult> {
    return this.http.post<ConflictResult>(`${this.apiUrl}/check-conflicts`, request);
  }

  categorize(request: CategorizationRequest): Observable<CategorizationResult> {
    return this.http.post<CategorizationResult>(`${this.apiUrl}/categorize`, request);
  }

  suggestSlots(duration?: number, days?: number): Observable<SlotSuggestion[]> {
    let params = new HttpParams();
    if (duration) params = params.set('duration', duration.toString());
    if (days) params = params.set('days', days.toString());
    return this.http.get<SlotSuggestion[]>(`${this.apiUrl}/suggest-slots`, { params });
  }

  getAnalytics(): Observable<AnalyticsResponse> {
    return this.http.get<AnalyticsResponse>(`${this.apiUrl}/analytics`);
  }
}