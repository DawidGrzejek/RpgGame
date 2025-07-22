import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notificationContainer: HTMLElement | null = null;

  constructor() {
    this.createNotificationContainer();
    this.addNotificationStyles();
  }

  showSuccess(message: string, title?: string): void {
    console.log(`SUCCESS: ${title ? title + ' - ' : ''}${message}`);
    this.showBrowserNotification(message, 'success');
  }

  showError(message: string, title?: string): void {
    console.error(`ERROR: ${title ? title + ' - ' : ''}${message}`);
    this.showBrowserNotification(message, 'error');
  }

  showWarning(message: string, title?: string): void {
    console.warn(`WARNING: ${title ? title + ' - ' : ''}${message}`);
    this.showBrowserNotification(message, 'warning');
  }

  showInfo(message: string, title?: string): void {
    console.info(`INFO: ${title ? title + ' - ' : ''}${message}`);
    this.showBrowserNotification(message, 'info');
  }

  private createNotificationContainer(): void {
    if (!this.notificationContainer) {
      this.notificationContainer = document.createElement('div');
      this.notificationContainer.className = 'notification-container';
      this.notificationContainer.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 10000;
        pointer-events: none;
      `;
      document.body.appendChild(this.notificationContainer);
    }
  }

  private addNotificationStyles(): void {
    if (!document.getElementById('notification-styles')) {
      const style = document.createElement('style');
      style.id = 'notification-styles';
      style.textContent = `
        @keyframes slideInRight {
          from {
            transform: translateX(100%);
            opacity: 0;
          }
          to {
            transform: translateX(0);
            opacity: 1;
          }
        }

        @keyframes slideOutRight {
          from {
            transform: translateX(0);
            opacity: 1;
          }
          to {
            transform: translateX(100%);
            opacity: 0;
          }
        }
      `;
      document.head.appendChild(style);
    }
  }

  private showBrowserNotification(message: string, type: 'success' | 'error' | 'warning' | 'info'): void {
    if (!this.notificationContainer) return;

    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;
    notification.style.cssText = `
      padding: 12px 16px;
      margin-bottom: 12px;
      border-radius: 4px;
      color: white;
      font-weight: 500;
      max-width: 300px;
      background-color: ${this.getNotificationColor(type)};
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
      animation: slideInRight 0.3s ease-out;
      pointer-events: auto;
      cursor: pointer;
    `;

    // Click to dismiss
    notification.addEventListener('click', () => {
      this.removeNotification(notification);
    });

    this.notificationContainer.appendChild(notification);

    // Auto remove after 5 seconds
    setTimeout(() => {
      this.removeNotification(notification);
    }, 5000);
  }

  private removeNotification(notification: HTMLElement): void {
    if (notification.parentNode) {
      notification.style.animation = 'slideOutRight 0.3s ease-in';
      setTimeout(() => {
        if (notification.parentNode) {
          notification.parentNode.removeChild(notification);
        }
      }, 300);
    }
  }

  private getNotificationColor(type: string): string {
    switch (type) {
      case 'success': return '#10b981';
      case 'error': return '#ef4444';
      case 'warning': return '#f59e0b';
      case 'info': return '#3b82f6';
      default: return '#6b7280';
    }
  }
}
