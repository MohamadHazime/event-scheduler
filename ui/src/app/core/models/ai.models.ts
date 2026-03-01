export interface ParseEventRequest {
  input: string;
}

export interface ParseEventResult {
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  location: string;
  category: string;
  parsed: boolean;
}

export interface GenerateDescriptionRequest {
  title: string;
  category?: string;
  location?: string;
}

export interface AiTextResult {
  text: string;
}

export interface SuggestTitleRequest {
  input: string;
}