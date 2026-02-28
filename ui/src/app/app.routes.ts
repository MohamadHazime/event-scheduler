import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';
import { LayoutComponent } from './shared/components/layout/layout.component';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'events', loadComponent: () => import('./features/events/event-list/event-list.component').then(m => m.EventListComponent) },
      { path: 'events/new', loadComponent: () => import('./features/events/event-form/event-form.component').then(m => m.EventFormComponent) },
      { path: 'events/:id', loadComponent: () => import('./features/events/event-detail/event-detail.component').then(m => m.EventDetailComponent) },
      { path: 'events/:id/edit', loadComponent: () => import('./features/events/event-form/event-form.component').then(m => m.EventFormComponent) },
      { path: 'invitations', loadComponent: () => import('./features/invitations/invitations.component').then(m => m.InvitationsComponent) },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'login' }
];