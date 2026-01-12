import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

export interface ConfirmDeleteDialogData {
  taskName: string;
}

@Component({
  selector: 'app-confirm-delete-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  templateUrl: './confirm-delete-dialog.component.html',
  styleUrls: ['./confirm-delete-dialog.component.scss']
})
export class ConfirmDeleteDialogComponent {
  readonly taskName: string;

  private readonly dialogRef = inject(MatDialogRef<ConfirmDeleteDialogComponent>);
  private readonly data = inject<ConfirmDeleteDialogData>(MAT_DIALOG_DATA);

  constructor() {
    this.taskName = this.data?.taskName || 'this task';
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  confirm(): void {
    this.dialogRef.close(true);
  }
}
