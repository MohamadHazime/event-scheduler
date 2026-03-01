import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { EventResponse, PagedResult } from '../../../core/models/event.models';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './event-list.component.html'
})
export class EventListComponent implements OnInit {
  private eventService = inject(EventService);
  private authService = inject(AuthService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);

  events: EventResponse[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 9;
  totalPages = 0;
  loading = false;
  filtersOpen = false;

  filterForm: FormGroup = this.fb.group({
    title: [''],
    location: [''],
    category: [''],
    fromDate: [''],
    toDate: ['']
  });

  categories = ['Meeting', 'Social', 'Health', 'Work', 'Personal', 'Other'];

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.loading = true;
    const filters = this.filterForm.value;

    this.eventService.getEvents({
      title: filters.title || undefined,
      location: filters.location || undefined,
      category: filters.category || undefined,
      fromDate: filters.fromDate || undefined,
      toDate: filters.toDate || undefined,
      page: this.page,
      pageSize: this.pageSize
    }).subscribe({
      next: (result: PagedResult<EventResponse>) => {
        this.events = result.items;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
        this.loading = false;
      },
      error: () => {
        this.toast.error('Failed to load events.');
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.page = 1;
    this.loadEvents();
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.page = 1;
    this.loadEvents();
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages) return;
    this.page = p;
    this.loadEvents();
  }

  deleteEvent(id: string, title: string): void {
    if (!confirm(`Are you sure you want to delete "${title}"?`)) return;

    this.eventService.deleteEvent(id).subscribe({
      next: () => {
        this.toast.success('Event deleted successfully.');
        this.loadEvents();
      },
      error: (err) => {
        this.toast.error(err.error?.error || 'Failed to delete event.');
      }
    });
  }

  isOwner(event: EventResponse): boolean {
    return event.createdById === this.authService.user()?.id;
  }

  getCategoryColor(category: string): string {
    const colors: Record<string, string> = {
      'Meeting': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
      'Social': 'bg-pink-100 text-pink-800 dark:bg-pink-900 dark:text-pink-200',
      'Health': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
      'Work': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
      'Personal': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
      'Other': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200'
    };
    return colors[category] || colors['Other'];
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      weekday: 'short', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
    });
  }

  getPages(): number[] {
    const pages: number[] = [];
    const start = Math.max(1, this.page - 2);
    const end = Math.min(this.totalPages, this.page + 2);
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  }
}