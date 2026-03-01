import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { InvitationService } from '../../core/services/invitation.service';
import { ToastService } from '../../core/services/toast.service';
import { InvitationResponse } from '../../core/models/invitation.models';

@Component({
  selector: 'app-invitations',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './invitations.component.html'
})
export class InvitationsComponent implements OnInit {
  private invitationService = inject(InvitationService);
  private toast = inject(ToastService);

  pendingInvitations: InvitationResponse[] = [];
  sentInvitations: InvitationResponse[] = [];
  activeTab: 'received' | 'sent' = 'received';
  loading = true;

  ngOnInit(): void {
    this.loadInvitations();
  }

  loadInvitations(): void {
    this.loading = true;
    let loaded = 0;
    const done = () => { loaded++; if (loaded === 2) this.loading = false; };

    this.invitationService.getPendingInvitations().subscribe({
      next: (inv) => { this.pendingInvitations = inv; done(); },
      error: () => { this.toast.error('Failed to load received invitations.'); done(); }
    });

    this.invitationService.getSentInvitations().subscribe({
      next: (inv) => { this.sentInvitations = inv; done(); },
      error: () => { this.toast.error('Failed to load sent invitations.'); done(); }
    });
  }

  accept(token: string): void {
    this.invitationService.acceptInvitation(token).subscribe({
      next: () => {
        this.toast.success('Invitation accepted! You are now attending this event.');
        this.loadInvitations();
      },
      error: (err) => this.toast.error(err.error?.error || 'Failed to accept invitation.')
    });
  }

  decline(token: string): void {
    if (!confirm('Are you sure you want to decline this invitation?')) return;
    this.invitationService.declineInvitation(token).subscribe({
      next: () => {
        this.toast.success('Invitation declined.');
        this.loadInvitations();
      },
      error: (err) => this.toast.error(err.error?.error || 'Failed to decline invitation.')
    });
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      month: 'short', day: 'numeric', year: 'numeric', hour: '2-digit', minute: '2-digit'
    });
  }

  getStatusBadge(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'badge-pending',
      'Accepted': 'badge-attending',
      'Declined': 'badge-declined',
      'Expired': 'bg-gray-100 text-gray-500 dark:bg-gray-700 dark:text-gray-400'
    };
    return map[status] || 'badge-pending';
  }
}