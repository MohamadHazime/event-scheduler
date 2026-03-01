import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { SmartService } from '../../../core/services/smart.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConflictingEvent } from '../../../core/models/smart.models';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './event-form.component.html'
})
export class EventFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private eventService = inject(EventService);
  private smartService = inject(SmartService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  form: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(300)]],
    description: ['', [Validators.maxLength(2000)]],
    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]],
    location: ['', [Validators.maxLength(500)]],
    category: ['Other']
  });

  isEditMode = false;
  eventId: string | null = null;
  loading = false;
  loadingEvent = false;
  categories = ['Meeting', 'Social', 'Health', 'Work', 'Personal', 'Other'];
  conflicts: ConflictingEvent[] = [];
  categorizingAi = false;

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id');
    if (this.eventId) {
      this.isEditMode = true;
      this.loadEvent();
    }
  }

  loadEvent(): void {
    if (!this.eventId) return;
    this.loadingEvent = true;

    this.eventService.getEventById(this.eventId).subscribe({
      next: (event) => {
        this.form.patchValue({
          title: event.title,
          description: event.description,
          startDate: this.toLocalDatetime(event.startDate),
          endDate: this.toLocalDatetime(event.endDate),
          location: event.location,
          category: event.category
        });
        this.loadingEvent = false;
      },
      error: () => {
        this.toast.error('Failed to load event.');
        this.router.navigate(['/events']);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const value = this.form.value;

    const request = {
      title: value.title,
      description: value.description || '',
      startDate: new Date(value.startDate).toISOString(),
      endDate: new Date(value.endDate).toISOString(),
      location: value.location || '',
      category: value.category
    };

    const obs = this.isEditMode
      ? this.eventService.updateEvent(this.eventId!, request)
      : this.eventService.createEvent(request);

    obs.subscribe({
      next: (event) => {
        this.toast.success(this.isEditMode ? 'Event updated!' : 'Event created!');
        this.router.navigate(['/events', event.id]);
      },
      error: (err) => {
        this.loading = false;
        this.toast.error(err.error?.error || 'Failed to save event.');
      }
    });
  }

  checkConflicts(): void {
    const { startDate, endDate } = this.form.value;
    if (!startDate || !endDate) return;

    this.smartService.checkConflicts({
      startDate: new Date(startDate).toISOString(),
      endDate: new Date(endDate).toISOString(),
      excludeEventId: this.eventId || undefined
    }).subscribe({
      next: (result) => {
        this.conflicts = result.conflicts;
        if (result.hasConflicts) {
          this.toast.warning(`${result.conflicts.length} conflict(s) found!`);
        } else {
          this.toast.success('No conflicts found.');
        }
      },
      error: () => this.toast.error('Failed to check conflicts.')
    });
  }

  autoCategorize(): void {
    const { title, description } = this.form.value;
    if (!title) return;

    this.categorizingAi = true;
    this.smartService.categorize({ title, description }).subscribe({
      next: (result) => {
        this.form.patchValue({ category: result.suggestedCategory });
        this.categorizingAi = false;
        this.toast.info(`Suggested category: ${result.suggestedCategory}`);
      },
      error: () => {
        this.categorizingAi = false;
        this.toast.error('Failed to categorize.');
      }
    });
  }

  formatConflictDate(dateStr: string): string {
    return new Date(dateStr).toLocaleString('en-US', {
      month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
    });
  }

  private toLocalDatetime(dateStr: string): string {
    const d = new Date(dateStr);
    const offset = d.getTimezoneOffset();
    const local = new Date(d.getTime() - offset * 60 * 1000);
    return local.toISOString().slice(0, 16);
  }
}