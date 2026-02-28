import { Component, inject } from '@angular/core';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  templateUrl: './toast.component.html'
})
export class ToastComponent {
  toastService = inject(ToastService);

  getIcon(type: string): string {
    switch (type) {
      case 'success': return '✓';
      case 'error': return '✕';
      case 'warning': return '⚠';
      default: return 'ℹ';
    }
  }

  getClasses(type: string): string {
    switch (type) {
      case 'success': return 'bg-green-50 border-green-500 text-green-800 dark:bg-green-900/50 dark:border-green-400 dark:text-green-200';
      case 'error': return 'bg-red-50 border-red-500 text-red-800 dark:bg-red-900/50 dark:border-red-400 dark:text-red-200';
      case 'warning': return 'bg-yellow-50 border-yellow-500 text-yellow-800 dark:bg-yellow-900/50 dark:border-yellow-400 dark:text-yellow-200';
      default: return 'bg-blue-50 border-blue-500 text-blue-800 dark:bg-blue-900/50 dark:border-blue-400 dark:text-blue-200';
    }
  }
}