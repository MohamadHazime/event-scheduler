import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { SmartService } from '../../../core/services/smart.service';
import { AiService } from '../../../core/services/ai.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConflictingEvent } from '../../../core/models/smart.models';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, FormsModule],
  templateUrl: './event-form.component.html'
})
export class EventFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private eventService = inject(EventService);
  private smartService = inject(SmartService);
  private aiService = inject(AiService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  form: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(300)]],
    description: ['', [Validators.maxLength(2000)]],
    startDate: ['', [Validators.required, this.pastDateValidator]],
    endDate: ['', [Validators.required, this.pastDateValidator]],
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
  generatingDescription = false;
  suggestingTitle = false;
  showAiParser = false;
  aiInput = '';
  parsingAi = false;
  minDateTime: string = this.getMinDateTime();

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id');
    if (this.eventId) {
      this.isEditMode = true;
      this.loadEvent();
    } else {
      const start = this.route.snapshot.queryParamMap.get('start');
      const end = this.route.snapshot.queryParamMap.get('end');
      if (start && end) {
        this.form.patchValue({
          startDate: this.toLocalDatetime(start),
          endDate: this.toLocalDatetime(end)
        });
      }
    }

    this.form.get('startDate')?.valueChanges.subscribe(startValue => {
      const endValue = this.form.get('endDate')?.value;
      if (startValue && endValue && endValue <= startValue) {
        this.form.patchValue({ endDate: '' });
      }
      this.form.get('endDate')?.updateValueAndValidity();
    });
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

  // --- AI Features ---

  parseWithAi(): void {
    if (!this.aiInput.trim()) return;

    this.parsingAi = true;
    this.aiService.parseEvent({ input: this.aiInput }).subscribe({
      next: (result) => {
        this.form.patchValue({
          title: result.title,
          description: result.description,
          startDate: this.toLocalDatetime(result.startDate),
          endDate: this.toLocalDatetime(result.endDate),
          location: result.location,
          category: result.category
        });
        this.parsingAi = false;
        this.showAiParser = false;
        this.aiInput = '';
        this.toast.success('AI filled in the event details!');
      },
      error: (err) => {
        this.parsingAi = false;
        this.toast.error(err.error?.error || 'AI parsing failed.');
      }
    });
  }

  generateDescription(): void {
    const { title, category, location } = this.form.value;
    if (!title) { this.toast.warning('Enter a title first.'); return; }

    this.generatingDescription = true;
    this.aiService.generateDescription({ title, category, location }).subscribe({
      next: (result) => {
        this.form.patchValue({ description: result.text });
        this.generatingDescription = false;
        this.toast.success('AI generated description!');
      },
      error: () => {
        this.generatingDescription = false;
        this.toast.error('Failed to generate description.');
      }
    });
  }

  suggestTitle(): void {
    const title = this.form.value.title;
    if (!title) return;

    this.suggestingTitle = true;
    this.aiService.suggestTitle(title).subscribe({
      next: (result) => {
        this.form.patchValue({ title: result.text });
        this.suggestingTitle = false;
        this.toast.info('Title polished by AI!');
      },
      error: () => {
        this.suggestingTitle = false;
        this.toast.error('Failed to suggest title.');
      }
    });
  }

  autoCategorize(): void {
    const { title, description } = this.form.value;
    if (!title) return;

    this.categorizingAi = true;
    this.aiService.categorize(title, description).subscribe({
      next: (result) => {
        this.form.patchValue({ category: result.suggestedCategory });
        this.categorizingAi = false;
        this.toast.info(`AI suggested: ${result.suggestedCategory}`);
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

  private getMinDateTime(): string {
    const now = new Date();
    const offset = now.getTimezoneOffset();
    const local = new Date(now.getTime() - offset * 60 * 1000);
    return local.toISOString().slice(0, 16);
  }

  private pastDateValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;
    const selected = new Date(value);
    if (selected < new Date()) {
      return { pastDate: true };
    }
    return null;
  }
}