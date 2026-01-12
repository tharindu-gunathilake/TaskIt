import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  constructor(private readonly snackBar: MatSnackBar) {}

  showSuccess(message: string, duration: number = 3000): void {
    const config: MatSnackBarConfig = {
      duration,
      panelClass: ['success-snackbar'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    };
    this.snackBar.open(message, 'Close', config);
  }

  showError(message: string, duration: number = 5000): void {
    const config: MatSnackBarConfig = {
      duration,
      panelClass: ['error-snackbar'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    };
    this.snackBar.open(message, 'Close', config);
  }

  showInfo(message: string, duration: number = 3000): void {
    const config: MatSnackBarConfig = {
      duration,
      panelClass: ['info-snackbar'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    };
    this.snackBar.open(message, 'Close', config);
  }
}
