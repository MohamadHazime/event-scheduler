import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateInvitationRequest, InvitationResponse } from '../models/invitation.models';

@Injectable({ providedIn: 'root' })
export class InvitationService {
  private readonly apiUrl = `${environment.apiUrl}/invitations`;

  constructor(private http: HttpClient) {}

  createInvitation(request: CreateInvitationRequest): Observable<InvitationResponse> {
    return this.http.post<InvitationResponse>(this.apiUrl, request);
  }

  getPendingInvitations(): Observable<InvitationResponse[]> {
    return this.http.get<InvitationResponse[]>(`${this.apiUrl}/pending`);
  }

  getSentInvitations(): Observable<InvitationResponse[]> {
    return this.http.get<InvitationResponse[]>(`${this.apiUrl}/sent`);
  }

  getByToken(token: string): Observable<InvitationResponse> {
    return this.http.get<InvitationResponse>(`${this.apiUrl}/link/${token}`);
  }

  acceptInvitation(token: string): Observable<InvitationResponse> {
    return this.http.post<InvitationResponse>(`${this.apiUrl}/${token}/accept`, {});
  }

  declineInvitation(token: string): Observable<InvitationResponse> {
    return this.http.post<InvitationResponse>(`${this.apiUrl}/${token}/decline`, {});
  }
}