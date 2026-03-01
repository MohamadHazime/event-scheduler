import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SmartService } from '../../core/services/smart.service';
import { EventService } from '../../core/services/event.service';
import { InvitationService } from '../../core/services/invitation.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { AnalyticsResponse, SlotSuggestion } from '../../core/models/smart.models';
import { EventResponse } from '../../core/models/event.models';
import { InvitationResponse } from '../../core/models/invitation.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  private smartService = inject(SmartService);
  private eventService = inject(EventService);
  private invitationService = inject(InvitationService);
  authService = inject(AuthService);
  private toast = inject(ToastService);

  analytics: AnalyticsResponse | null = null;
  slots: SlotSuggestion[] = [];
  upcomingEvents: EventResponse[] = [];
  pendingInvitations: InvitationResponse[] = [];
  loading = true;

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;
    let loaded = 0;
    const total = 4;
    const done = () => { loaded++; if (loaded >= total) this.loading = false; };

    this.smartService.getAnalytics().subscribe({
      next: (data) => { this.analytics = data; done(); },
      error: () => done()
    });

    this.smartService.suggestSlots(60, 3).subscribe({
      next: (data) => { this.slots = data; done(); },
      error: () => done()
    });

    this.eventService.getEvents({ page: 1, pageSize: 5 }).subscribe({
      next: (data) => {
        this.upcomingEvents = data.items.filter(e => new Date(e.startDate) >= new Date());
        done();
      },
      error: () => done()
    });

    this.invitationService.getPendingInvitations().subscribe({
      next: (data) => { this.pendingInvitations = data; done(); },
      error: () => done()
    });
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      weekday: 'short', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
    });
  }

  formatShortDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
    });
  }

  getCategoryColor(category: string): string {
    const colors: Record<string, string> = {
      'Meeting': 'bg-blue-500', 'Social': 'bg-pink-500', 'Health': 'bg-green-500',
      'Work': 'bg-purple-500', 'Personal': 'bg-yellow-500', 'Other': 'bg-gray-500'
    };
    return colors[category] || 'bg-gray-500';
  }

  getBarWidth(count: number): number {
    if (!this.analytics) return 0;
    const max = Math.max(...this.analytics.eventsByCategory.map(c => c.count), 1);
    return (count / max) * 100;
  }

  get totalAttendance(): number {
    if (!this.analytics) return 0;
    const s = this.analytics.attendanceStats;
    return s.attending + s.maybe + s.declined + s.upcoming;
  }

  getAttendancePercent(count: number): number {
    return this.totalAttendance > 0 ? Math.round((count / this.totalAttendance) * 100) : 0;
  }
}