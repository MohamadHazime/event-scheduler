export interface ConflictCheckRequest {
  startDate: string;
  endDate: string;
  excludeEventId?: string;
}

export interface ConflictResult {
  hasConflicts: boolean;
  conflicts: ConflictingEvent[];
}

export interface ConflictingEvent {
  id: string;
  title: string;
  startDate: string;
  endDate: string;
  location: string;
}

export interface CategorizationRequest {
  title: string;
  description?: string;
}

export interface CategorizationResult {
  suggestedCategory: string;
  allCategories: string[];
}

export interface SlotSuggestion {
  start: string;
  end: string;
  label: string;
}

export interface AnalyticsResponse {
  totalEvents: number;
  upcomingEvents: number;
  eventsByCategory: CategoryCount[];
  eventsByMonth: MonthCount[];
  busiestDays: DayCount[];
  attendanceStats: AttendanceStats;
}

export interface CategoryCount {
  category: string;
  count: number;
}

export interface MonthCount {
  month: string;
  count: number;
}

export interface DayCount {
  day: string;
  count: number;
}

export interface AttendanceStats {
  attending: number;
  maybe: number;
  declined: number;
  upcoming: number;
}