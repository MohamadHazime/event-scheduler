# Event Scheduler

A full-stack event scheduling application with AI-powered features, built with .NET Core and Angular.

## Live Demo

- **Frontend:** https://event-scheduler-xxxx.onrender.com
- **Backend API:** https://event-scheduler-api-xxxx.onrender.com/swagger

## Features

### Core Features
- **Event Management:** Create, edit, delete events with title, date/time, location, description, and category
- **Status Tracking:** Mark events as Upcoming, Attending, Maybe, or Declined
- **Search & Filter:** Find events by title, date range, location, or category with pagination
- **User Accounts:** JWT-based authentication with registration and login
- **Invitation System:** Invite users by email with accept/decline functionality

### AI Features (Groq - Llama 3.3 70B)
- **Natural Language Event Creation:** Type "Team lunch Friday at noon at Pizza Hut" and AI fills all form fields
- **AI Description Generator:** One-click professional event description generation
- **AI Smart Categorization:** LLM-powered event categorization
- **AI Title Polish:** Cleans up rough titles into professional ones

### Smart Features
- **Conflict Detection:** Automatic time-overlap detection when creating events
- **Smart Scheduling Assistant:** AI suggests free time slots in your calendar
- **Analytics Dashboard:** Event statistics, category breakdown, attendance rates

### Extra Features
- Mobile-responsive design (Tailwind CSS)
- Toast notifications for all actions
- Category color coding

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Architecture | Clean Architecture + DDD + CQRS |
| Backend | .NET Core Web API (Minimal APIs) |
| ORM | Entity Framework Core |
| Database | PostgreSQL |
| Auth | JWT Bearer Tokens + BCrypt |
| CQRS | MediatR |
| Frontend | Angular 18 (Standalone, Reactive Forms) |
| Styling | Tailwind CSS + SCSS |
| AI | Groq (Llama 3.3 70B) |
| Deployment | Render.com |

## Running Locally

### Prerequisites
- .NET SDK 10.0
- Node.js 18+
- PostgreSQL
- Groq API key (free at https://console.groq.com)

### Backend

1. Update the connection string in `backend/src/EventScheduler.API/appsettings.json`
2. Run the following commands:

```bash
cd backend
dotnet restore
dotnet ef database update --project src/EventScheduler.Infrastructure --startup-project src/EventScheduler.API
dotnet run --project src/EventScheduler.API
```

### Frontend

```bash
cd frontend
npm install
ng serve
```

Open http://localhost:4200

## Project Structure

```
event-scheduler/
|-- backend/
|   |-- src/
|   |   |-- EventScheduler.Domain/          (Entities, Value Objects, Enums, Interfaces)
|   |   |-- EventScheduler.Application/     (CQRS Commands/Queries, DTOs, Interfaces)
|   |   |-- EventScheduler.Infrastructure/  (EF Core, Repositories, External Services)
|   |   |-- EventScheduler.API/             (Minimal API Endpoints, Program.cs)
|   |-- Dockerfile
|-- frontend/
|   |-- src/app/
|       |-- core/       (Services, Guards, Interceptors, Models)
|       |-- features/   (Auth, Events, Invitations, Dashboard)
|       |-- shared/     (Navbar, Toast, Layout)
|-- README.md
```

## API Endpoints

### Auth
- POST /api/auth/register - Register new user
- POST /api/auth/login - Login and get JWT token
- GET /api/auth/me - Get current user profile

### Events
- GET /api/events - List events (with search/filter/pagination)
- GET /api/events/{id} - Get event details
- POST /api/events - Create event
- PUT /api/events/{id} - Update event
- DELETE /api/events/{id} - Delete event
- PUT /api/events/{id}/status - Update attendance status
- GET /api/events/{id}/attendees - List attendees

### Invitations
- POST /api/invitations - Send invitation
- GET /api/invitations/pending - Get received invitations
- GET /api/invitations/sent - Get sent invitations
- GET /api/invitations/link/{token} - View invitation details
- POST /api/invitations/{token}/accept - Accept invitation
- POST /api/invitations/{token}/decline - Decline invitation

### Smart Features
- POST /api/smart/check-conflicts - Check time conflicts
- GET /api/smart/suggest-slots - Get free slot suggestions
- POST /api/smart/categorize - Keyword-based categorization
- GET /api/smart/analytics - Event analytics

### AI Features
- POST /api/ai/parse-event - Natural language to structured event
- POST /api/ai/generate-description - AI description generation
- POST /api/ai/categorize - AI-powered categorization
- POST /api/ai/suggest-title - Polish event title
