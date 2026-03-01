import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EventService } from '../../../core/services/event.service';
import { InvitationService } from '../../../core/services/invitation.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { EventResponse } from '../../../core/models/event.models';

@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './event-detail.component.html'
})
export class EventDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventService = inject(EventService);
  private invitationService = inject(InvitationService);
  private authService = inject(AuthService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);

  event: EventResponse | null = null;
  loading = true;
  showInviteForm = false;
  inviting = false;

  inviteForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  statuses = ['Attending', 'Maybe', 'Declined'];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadEvent(id);
  }

  loadEvent(id: string): void {
    this.loading = true;
    this.eventService.getEventById(id).subscribe({
      next: (event) => {
        this.event = event;
        this.loading = false;
      },
      error: () => {
        this.toast.error('Event not found.');
        this.router.navigate(['/events']);
      }
    });
  }

  get isOwner(): boolean {
    return this.event?.createdById === this.authService.user()?.id;
  }

  get currentUserStatus(): string | null {
    const userId = this.authService.user()?.id;
    const attendee = this.event?.attendees.find(a => a.userId === userId);
    return attendee?.status || null;
  }

  updateStatus(status: string): void {
    if (!this.event) return;
    this.eventService.updateStatus(this.event.id, status).subscribe({
      next: () => {
        this.toast.success(`Status updated to ${status}.`);
        this.loadEvent(this.event!.id);
      },
      error: (err) => this.toast.error(err.error?.error || 'Failed to update status.')
    });
  }

  deleteEvent(): void {
    if (!this.event || !confirm(`Delete "${this.event.title}"?`)) return;
    this.eventService.deleteEvent(this.event.id).subscribe({
      next: () => {
        this.toast.success('Event deleted.');
        this.router.navigate(['/events']);
      },
      error: (err) => this.toast.error(err.error?.error || 'Failed to delete event.')
    });
  }

  sendInvite(): void {
    if (this.inviteForm.invalid || !this.event) {
      this.inviteForm.markAllAsTouched();
      return;
    }

    this.inviting = true;
    this.invitationService.createInvitation({
      eventId: this.event.id,
      inviteeEmail: this.inviteForm.value.email
    }).subscribe({
      next: () => {
        this.toast.success('Invitation sent!');
        this.inviteForm.reset();
        this.showInviteForm = false;
        this.inviting = false;
      },
      error: (err) => {
        this.inviting = false;
        this.toast.error(err.error?.error || 'Failed to send invitation.');
      }
    });
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleString('en-US', {
      weekday: 'long', year: 'numeric', month: 'long', day: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  }

  getStatusBadge(status: string): string {
    const map: Record<string, string> = {
      'Attending': 'badge-attending',
      'Maybe': 'badge-maybe',
      'Declined': 'badge-declined',
      'Upcoming': 'badge-upcoming'
    };
    return map[status] || 'badge-pending';
  }
}