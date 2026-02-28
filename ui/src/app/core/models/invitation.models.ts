export interface CreateInvitationRequest {
  eventId: string;
  inviteeEmail: string;
}

export interface InvitationResponse {
  id: string;
  eventId: string;
  eventTitle: string;
  invitedByName: string;
  inviteeEmail: string;
  token: string;
  status: string;
  createdAt: string;
  expiresAt: string;
}