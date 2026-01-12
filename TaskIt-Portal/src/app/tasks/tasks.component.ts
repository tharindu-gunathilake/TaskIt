import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../services/auth.service';
import { TaskService } from '../services/task.service';
import { NotificationService } from '../services/notification.service';
import { TaskStatusEnum, UserTaskDto } from '../models/api.models';
import { ConfirmDeleteDialogComponent } from './confirm-delete-dialog.component';

interface StatusOption {
  label: string;
  value: TaskStatusEnum;
}

const STATUS_OPTIONS: StatusOption[] = [
  { label: 'Pending', value: TaskStatusEnum.Pending },
  { label: 'InProgress', value: TaskStatusEnum.InProgress },
  { label: 'Completed', value: TaskStatusEnum.Completed },
  { label: 'Archived', value: TaskStatusEnum.Archived }
];

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.scss']
})
export class TasksComponent implements OnInit {
  private readonly taskService = inject(TaskService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly notificationService = inject(NotificationService);
  private readonly dialog = inject(MatDialog);

  tasks = signal<UserTaskDto[]>([]);
  totalCount = signal<number>(0);
  loading = signal(false);
  editingTaskId = signal<string | null>(null);
  searchTerm = '';
  statusFilter = signal<TaskStatusEnum | null>(null);

  readonly user = computed(() => this.authService.currentUser);
  readonly statusOptions = STATUS_OPTIONS;

  readonly totalPages = computed(() => {
    const count = this.totalCount();
    const pageSize = this.pagination().pageSize;
    return count > 0 ? Math.ceil(count / pageSize) : 0;
  });

  readonly currentPage = computed(() => this.pagination().pageNumber);

  readonly pageRange = computed(() => {
    const count = this.totalCount();
    if (count === 0) {
      return { start: 0, end: 0, total: 0 };
    }
    const pageSize = this.pagination().pageSize;
    const pageNumber = this.pagination().pageNumber;
    const start = (pageNumber - 1) * pageSize + 1;
    const end = Math.min(pageNumber * pageSize, count);
    return { start, end, total: count };
  });

  readonly filterOptions = [
    { label: 'All', value: null },
    ...STATUS_OPTIONS
  ];

  getStatusLabel(status: TaskStatusEnum): string {
    const option = STATUS_OPTIONS.find(opt => opt.value === status);
    return option?.label ?? TaskStatusEnum[status] ?? 'Unknown';
  }

  pagination = signal({
    pageNumber: 1,
    pageSize: 10,
    sortField: '',
    sortOrder: 'asc'
  });

  form = this.fb.group({
    name: ['', Validators.required],
    description: [''],
    status: [TaskStatusEnum.Pending, Validators.required]
  });

  ngOnInit(): void {
    if (!this.user()) {
      this.router.navigate(['/login']);
      return;
    }

    this.loadTasks();
  }

  loadTasks(): void {
    const user = this.user();
    if (!user) {
      return;
    }

    this.loading.set(true);
    const pagination = { 
      ...this.pagination(), 
      searchValue: this.searchTerm,
      status: this.statusFilter() ?? undefined
    };
    
    this.taskService.getTasks(pagination, user.id).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (!res.success || !res.data) {
          this.notificationService.showError(res.message ?? 'Failed to load tasks');
          this.tasks.set([]);
          this.totalCount.set(0);
          return;
        }
        this.tasks.set(res.data.userTasks);
        this.totalCount.set(res.data.totalCount);
      },
      error: (err) => {
        this.loading.set(false);
        this.notificationService.showError(err?.error?.message ?? 'Failed to load tasks');
      }
    });
  }

  resetForm(): void {
    this.editingTaskId.set(null);
    this.form.reset({
      name: '',
      description: '',
      status: TaskStatusEnum.Pending
    });
  }

  onSearchChange(term: string): void {
    this.searchTerm = term;
    this.pagination.update((state) => ({ ...state, pageNumber: 1 }));
    this.loadTasks();
  }

  onStatusFilterChange(status: TaskStatusEnum | null): void {
    this.statusFilter.set(status);
    this.pagination.update((state) => ({ ...state, pageNumber: 1 }));
    this.loadTasks();
  }

  sortBy(field: string): void {
    const currentPagination = this.pagination();
    
    if (currentPagination.sortField === field) {
      // Toggle sort order if clicking the same field
      const newOrder = currentPagination.sortOrder === 'asc' ? 'desc' : 'asc';
      this.pagination.update((state) => ({
        ...state,
        sortOrder: newOrder,
        pageNumber: 1
      }));
    } else {
      // Set new sort field and reset to 'asc'
      this.pagination.update((state) => ({
        ...state,
        sortField: field,
        sortOrder: 'asc',
        pageNumber: 1
      }));
    }
    
    this.loadTasks();
  }

  isSortedBy(field: string): boolean {
    return this.pagination().sortField === field;
  }

  getSortIcon(field: string): string {
    if (!this.isSortedBy(field)) {
      return '';
    }
    return this.pagination().sortOrder === 'asc' ? '↑' : '↓';
  }

  goToPage(page: number): void {
    const total = this.totalPages();
    if (page < 1 || page > total) {
      return;
    }
    this.pagination.update((state) => ({ ...state, pageNumber: page }));
    this.loadTasks();
  }

  goToFirstPage(): void {
    this.goToPage(1);
  }

  goToLastPage(): void {
    this.goToPage(this.totalPages());
  }

  goToNextPage(): void {
    const current = this.currentPage();
    const total = this.totalPages();
    if (current < total) {
      this.goToPage(current + 1);
    }
  }

  goToPreviousPage(): void {
    const current = this.currentPage();
    if (current > 1) {
      this.goToPage(current - 1);
    }
  }

  getPageNumbers(): (number | string)[] {
    const current = this.currentPage();
    const total = this.totalPages();
    const pages: (number | string)[] = [];

    if (total <= 7) {
      // Show all pages if 7 or fewer
      for (let i = 1; i <= total; i++) {
        pages.push(i);
      }
    } else {
      // Always show first page
      pages.push(1);

      if (current <= 4) {
        // Near the beginning
        for (let i = 2; i <= 5; i++) {
          pages.push(i);
        }
        pages.push('...');
        pages.push(total);
      } else if (current >= total - 3) {
        // Near the end
        pages.push('...');
        for (let i = total - 4; i <= total; i++) {
          pages.push(i);
        }
      } else {
        // In the middle
        pages.push('...');
        for (let i = current - 1; i <= current + 1; i++) {
          pages.push(i);
        }
        pages.push('...');
        pages.push(total);
      }
    }

    return pages;
  }

  startEdit(task: UserTaskDto): void {
    this.editingTaskId.set(task.id);
    this.form.setValue({
      name: task.name,
      description: task.description ?? '',
      status: task.status
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.value;
    const payload = {
      name: formValue.name!,
      description: formValue.description ?? null,
      status: typeof formValue.status === 'string' ? parseInt(formValue.status, 10) : formValue.status as TaskStatusEnum
    };
    const taskId = this.editingTaskId();

    this.loading.set(true);

    const request$ = taskId
      ? this.taskService.updateTask(taskId, payload)
      : this.taskService.createTask(payload);

    request$.subscribe({
      next: (res) => {
        this.loading.set(false);
        if (!res.success) {
          this.notificationService.showError(res.message ?? 'Save failed');
          return;
        }
        const message = taskId ? 'Task updated successfully' : 'Task created successfully';
        this.notificationService.showSuccess(message);
        this.resetForm();
        this.loadTasks();
      },
      error: (err) => {
        this.loading.set(false);
        this.notificationService.showError(err?.error?.message ?? 'Save failed');
      }
    });
  }

  delete(task: UserTaskDto): void {
    if (!task.id) {
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDeleteDialogComponent, {
      width: '400px',
      data: { taskName: task.name }
    });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.loading.set(true);
        this.taskService.deleteTask(task.id).subscribe({
          next: (res) => {
            this.loading.set(false);
            if (!res.success) {
              this.notificationService.showError(res.message ?? 'Delete failed');
              return;
            }
            this.notificationService.showSuccess('Task deleted successfully');
            this.loadTasks();
          },
          error: (err) => {
            this.loading.set(false);
            this.notificationService.showError(err?.error?.message ?? 'Delete failed');
          }
        });
      }
    });
  }
}
