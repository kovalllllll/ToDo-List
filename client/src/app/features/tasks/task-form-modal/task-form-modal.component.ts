import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { provideNativeDateAdapter } from '@angular/material/core';
import {
  ALL_TASK_PRIORITIES,
  ALL_TASK_STATUSES,
  TaskItem,
  TaskItemStatus,
  TaskPriority,
  TASK_PRIORITY_LABELS,
  TASK_STATUS_LABELS,
} from '../../../core/models/task.models';

export interface TaskFormDialogData {
  projectId: string;
  task?: TaskItem;
}

export const TASK_TITLE_MAX_LENGTH = 50;
export const TASK_DESCRIPTION_MAX_LENGTH = 200;

const ALL_HOURS = Array.from({ length: 24 }, (_, index) => index);
const ALL_MINUTES = Array.from({ length: 60 }, (_, index) => index);

function isSameDay(first: Date, second: Date): boolean {
  return (
    first.getFullYear() === second.getFullYear() &&
    first.getMonth() === second.getMonth() &&
    first.getDate() === second.getDate()
  );
}

function getAvailableHours(date: Date | null): number[] {
  if (!date) {
    return [];
  }

  const now = new Date();
  if (!isSameDay(date, now)) {
    return ALL_HOURS;
  }

  const currentHour = now.getHours();
  const currentMinute = now.getMinutes();

  return ALL_HOURS.filter((hour) => {
    if (hour > currentHour) {
      return true;
    }

    if (hour < currentHour) {
      return false;
    }

    return currentMinute < 59;
  });
}

function getAvailableMinutes(date: Date | null, hour: number | null): number[] {
  if (!date || hour === null) {
    return [];
  }

  const now = new Date();
  if (!isSameDay(date, now) || hour > now.getHours()) {
    return ALL_MINUTES;
  }

  if (hour < now.getHours()) {
    return [];
  }

  return ALL_MINUTES.filter((minute) => minute > now.getMinutes());
}

function futureDeadlineValidator(group: AbstractControl): ValidationErrors | null {
  const date = group.get('deadlineDate')?.value as Date | null;
  if (!date) {
    return null;
  }

  const availableHours = getAvailableHours(date);
  if (availableHours.length === 0) {
    return { noAvailableTimes: true };
  }

  const hour = group.get('deadlineHour')?.value as number | null;
  const minute = group.get('deadlineMinute')?.value as number | null;

  if (hour === null || minute === null) {
    return { futureDeadline: true };
  }

  const availableMinutes = getAvailableMinutes(date, hour);
  if (!availableMinutes.includes(minute)) {
    return { futureDeadline: true };
  }

  const deadline = new Date(date);
  deadline.setHours(hour, minute, 0, 0);

  return deadline.getTime() <= Date.now() ? { futureDeadline: true } : null;
}

function combineDeadline(
  date: Date | null,
  hour: number | null,
  minute: number | null
): string | null {
  if (!date || hour === null || minute === null) {
    return null;
  }

  const deadline = new Date(date);
  deadline.setHours(hour, minute, 0, 0);
  return deadline.toISOString();
}

@Component({
  selector: 'app-task-form-modal',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatIconModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './task-form-modal.component.html',
  styleUrl: './task-form-modal.component.scss',
})
export class TaskFormModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<TaskFormModalComponent>);
  readonly data = inject<TaskFormDialogData>(MAT_DIALOG_DATA);

  readonly isEdit = !!this.data.task;
  readonly statuses = ALL_TASK_STATUSES;
  readonly priorities = ALL_TASK_PRIORITIES;
  readonly statusLabels = TASK_STATUS_LABELS;
  readonly priorityLabels = TASK_PRIORITY_LABELS;
  readonly titleMaxLength = TASK_TITLE_MAX_LENGTH;
  readonly descriptionMaxLength = TASK_DESCRIPTION_MAX_LENGTH;
  readonly minDeadlineDate = new Date();
  readonly formatTimePart = (value: number) => String(value).padStart(2, '0');

  availableHours: number[] = [];
  availableMinutes: number[] = [];

  readonly form = this.fb.group(
    {
      title: this.fb.nonNullable.control('', [
        Validators.required,
        Validators.maxLength(TASK_TITLE_MAX_LENGTH),
      ]),
      description: this.fb.nonNullable.control('', [
        Validators.maxLength(TASK_DESCRIPTION_MAX_LENGTH),
      ]),
      status: this.fb.nonNullable.control(TaskItemStatus.Todo, Validators.required),
      priority: this.fb.control<TaskPriority | null>(null),
      deadlineDate: this.fb.control<Date | null>(null),
      deadlineHour: this.fb.control<number | null>(null),
      deadlineMinute: this.fb.control<number | null>(null),
    },
    { validators: [futureDeadlineValidator] }
  );

  constructor() {
    this.form.controls.deadlineDate.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.syncDeadlineTime());

    this.form.controls.deadlineHour.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.syncDeadlineMinutes());
  }

  ngOnInit(): void {
    if (this.data.task) {
      const deadline = this.data.task.deadlineUtc ? new Date(this.data.task.deadlineUtc) : null;

      this.form.patchValue({
        title: this.data.task.title,
        description: this.data.task.description ?? '',
        status: this.data.task.status,
        priority: this.data.task.priority,
        deadlineDate: deadline,
        deadlineHour: deadline?.getHours() ?? null,
        deadlineMinute: deadline?.getMinutes() ?? null,
      });
    }

    this.syncDeadlineTime();
  }

  cancel(): void {
    this.dialogRef.close();
  }

  clearDeadline(): void {
    this.form.patchValue({
      deadlineDate: null,
      deadlineHour: null,
      deadlineMinute: null,
    });
    this.syncDeadlineTime();
    this.form.controls.deadlineDate.markAsTouched();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.dialogRef.close({
      title: value.title.trim(),
      description: value.description.trim() || null,
      status: value.status,
      priority: value.priority || null,
      deadlineUtc: combineDeadline(value.deadlineDate, value.deadlineHour, value.deadlineMinute),
      projectId: this.data.projectId,
    });
  }

  hasFutureDeadlineError(): boolean {
    return (
      this.form.hasError('futureDeadline') &&
      (this.form.controls.deadlineDate.touched ||
        this.form.controls.deadlineHour.touched ||
        this.form.controls.deadlineMinute.touched)
    );
  }

  hasNoAvailableTimes(): boolean {
    return (
      !!this.form.controls.deadlineDate.value &&
      this.availableHours.length === 0 &&
      this.form.controls.deadlineDate.touched
    );
  }

  private syncDeadlineTime(): void {
    const date = this.form.controls.deadlineDate.value;
    this.availableHours = getAvailableHours(date);

    if (!date) {
      this.form.controls.deadlineHour.disable({ emitEvent: false });
      this.form.controls.deadlineMinute.disable({ emitEvent: false });
      this.form.controls.deadlineHour.setValue(null, { emitEvent: false });
      this.form.controls.deadlineMinute.setValue(null, { emitEvent: false });
      this.availableMinutes = [];
      this.form.updateValueAndValidity({ emitEvent: false });
      return;
    }

    this.form.controls.deadlineHour.enable({ emitEvent: false });
    this.form.controls.deadlineMinute.enable({ emitEvent: false });

    const currentHour = this.form.controls.deadlineHour.value;
    if (currentHour === null || !this.availableHours.includes(currentHour)) {
      this.form.controls.deadlineHour.setValue(this.availableHours[0] ?? null, { emitEvent: false });
    }

    this.syncDeadlineMinutes(false);
  }

  private syncDeadlineMinutes(updateValidity = true): void {
    const date = this.form.controls.deadlineDate.value;
    const hour = this.form.controls.deadlineHour.value;

    this.availableMinutes = getAvailableMinutes(date, hour);

    if (!date || hour === null) {
      this.form.controls.deadlineMinute.setValue(null, { emitEvent: false });
      if (updateValidity) {
        this.form.updateValueAndValidity({ emitEvent: false });
      }
      return;
    }

    const currentMinute = this.form.controls.deadlineMinute.value;
    if (currentMinute === null || !this.availableMinutes.includes(currentMinute)) {
      this.form.controls.deadlineMinute.setValue(this.availableMinutes[0] ?? null, {
        emitEvent: false,
      });
    }

    if (updateValidity) {
      this.form.updateValueAndValidity({ emitEvent: false });
    }
  }
}
