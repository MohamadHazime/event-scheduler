import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ParseEventRequest,
  ParseEventResult,
  GenerateDescriptionRequest,
  AiTextResult,
  SuggestTitleRequest
} from '../models/ai.models';
import { CategorizationResult } from '../models/smart.models';

@Injectable({ providedIn: 'root' })
export class AiService {
  private readonly apiUrl = `${environment.apiUrl}/ai`;

  constructor(private http: HttpClient) {}

  parseEvent(request: ParseEventRequest): Observable<ParseEventResult> {
    return this.http.post<ParseEventResult>(`${this.apiUrl}/parse-event`, request);
  }

  generateDescription(request: GenerateDescriptionRequest): Observable<AiTextResult> {
    return this.http.post<AiTextResult>(`${this.apiUrl}/generate-description`, request);
  }

  categorize(title: string, description?: string): Observable<CategorizationResult> {
    return this.http.post<CategorizationResult>(`${this.apiUrl}/categorize`, { title, description });
  }

  suggestTitle(input: string): Observable<AiTextResult> {
    return this.http.post<AiTextResult>(`${this.apiUrl}/suggest-title`, { input });
  }
}