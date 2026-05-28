import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import {
  ALL_TASK_PRIORITIES,
  ALL_TASK_STATUSES,
  DEFAULT_TASK_FILTERS,
  TaskFilters,
  TASK_PRIORITY_LABELS,
  TASK_STATUS_LABELS,
} from '../../../core/models/task.models';

@Component({
  selector: 'app-task-filters',
  standalone: true,
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './task-filters.component.html',
  styleUrl: './task-filters.component.scss',
})
export class TaskFiltersComponent implements OnInit {
  @Output() readonly filtersChange = new EventEmitter<TaskFilters>();

  readonly statuses = ALL_TASK_STATUSES;
  readonly priorities = ALL_TASK_PRIORITIES;
  readonly statusLabels = TASK_STATUS_LABELS;
  readonly priorityLabels = TASK_PRIORITY_LABELS;

  private readonly fb = new FormBuilder();

  readonly form = this.fb.nonNullable.group({
    search: DEFAULT_TASK_FILTERS.search,
    statuses: [DEFAULT_TASK_FILTERS.statuses],
    priorities: [DEFAULT_TASK_FILTERS.priorities],
  });

  constructor() {
    this.form.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => this.emitFilters());
  }

  ngOnInit(): void {
    this.emitFilters();
  }

  reset(): void {
    this.form.reset({
      search: DEFAULT_TASK_FILTERS.search,
      statuses: DEFAULT_TASK_FILTERS.statuses,
      priorities: DEFAULT_TASK_FILTERS.priorities,
    });
  }

  private emitFilters(): void {
    const value = this.form.getRawValue();
    this.filtersChange.emit({
      search: value.search,
      statuses: value.statuses ?? [],
      priorities: value.priorities ?? [],
    });
  }
}
