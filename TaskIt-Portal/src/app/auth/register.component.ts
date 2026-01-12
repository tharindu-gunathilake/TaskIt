import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);

  loading = signal(false);
  showPassword = signal(false);

  form = this.fb.group({
    userName: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(12)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  togglePasswordVisibility(): void {
    this.showPassword.update(v => !v);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.authService.register(this.form.value as any).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (!res.success) {
          this.notificationService.showError(res.message ?? 'Registration failed');
          return;
        }
        this.notificationService.showSuccess('Registration successful! Welcome to TaskIt.');
        this.router.navigate(['/tasks']);
      },
      error: (err) => {
        this.loading.set(false);
        this.notificationService.showError(err?.error?.message ?? 'Registration failed');
      }
    });
  }
}
