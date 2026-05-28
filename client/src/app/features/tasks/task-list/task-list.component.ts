import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Store } from '@ngrx/store';
import { Actions, ofType } from '@ngrx/effects';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  ALL_TASK_STATUSES,
  TaskFilters,
  TaskItem,
  TASK_PRIORITY_LABELS,
  TASK_STATUS_LABELS,
  UpdateTaskRequest,
} from '../../../core/models/task.models';
import { ProjectsActions } from '../../../store/projects/projects.actions';
import { selectSelectedProject } from '../../../store/projects/projects.selectors';
import { TasksActions } from '../../../store/tasks/tasks.actions';
import {
  selectAllTasks,
  selectTaskFilters,
  selectTasksError,
  selectTasksLoading,
} from '../../../store/tasks/tasks.selectors';
import { RelativeTimePipe } from '../../../shared/pipes/relative-time.pipe';
import { TaskFiltersComponent } from '../task-filters/task-filters.component';
import { TaskFormDialogData, TaskFormModalComponent } from '../task-form-modal/task-form-modal.component';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
    RouterLink,
    TaskFiltersComponent,
    RelativeTimePipe,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss',
})
export class TaskListComponent implements OnInit, OnDestroy {
  private readonly store = inject(Store);
  private readonly route = inject(ActivatedRoute);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly actions$ = inject(Actions);

  readonly project = this.store.selectSignal(selectSelectedProject);
  readonly tasks = this.store.selectSignal(selectAllTasks);
  readonly filters = this.store.selectSignal(selectTaskFilters);
  readonly loading = this.store.selectSignal(selectTasksLoading);
  readonly error = this.store.selectSignal(selectTasksError);

  readonly statusLabels = TASK_STATUS_LABELS;
  readonly priorityLabels = TASK_PRIORITY_LABELS;
  readonly statuses = ALL_TASK_STATUSES;
  readonly displayedColumns = ['title', 'status', 'priority', 'deadline', 'actions'];

  private projectId = '';

  constructor() {
    this.actions$
      .pipe(
        ofType(
          TasksActions.createTaskFailure,
          TasksActions.updateTaskFailure,
          TasksActions.deleteTaskFailure
        ),
        takeUntilDestroyed()
      )
      .subscribe(({ error }) => this.snackBar.open(error, 'Close', { duration: 5000 }));

    this.actions$
      .pipe(
        ofType(
          TasksActions.createTaskSuccess,
          TasksActions.updateTaskSuccess,
          TasksActions.deleteTaskSuccess
        ),
        takeUntilDestroyed()
      )
      .subscribe(() => this.snackBar.open('Task updated', 'Close', { duration: 3000 }));
  }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    this.store.dispatch(ProjectsActions.loadById({ projectId: this.projectId }));
    this.loadTasks(this.filters());
  }

  ngOnDestroy(): void {
    this.store.dispatch(TasksActions.clearTasks());
    this.store.dispatch(ProjectsActions.clearSelectedProject());
  }

  onFiltersChange(filters: TaskFilters): void {
    this.store.dispatch(TasksActions.setFilters({ filters }));
    this.loadTasks(filters);
  }

  openCreateDialog(): void {
    this.openTaskDialog({ projectId: this.projectId });
  }

  openEditDialog(task: TaskItem): void {
    this.openTaskDialog({ projectId: this.projectId, task });
  }

  deleteTask(task: TaskItem): void {
    if (confirm(`Delete task "${task.title}"?`)) {
      this.store.dispatch(TasksActions.deleteTask({ taskId: task.id }));
    }
  }

  getPriorityLabel(task: TaskItem): string {
    return task.priority ? this.priorityLabels[task.priority] : '—';
  }

  updateStatus(task: TaskItem, status: TaskItem['status']): void {
    if (task.status === status) {
      return;
    }

    const request: UpdateTaskRequest = {
      title: task.title,
      description: task.description,
      status,
      priority: task.priority,
      deadlineUtc: task.deadlineUtc,
    };

    this.store.dispatch(TasksActions.updateTask({ taskId: task.id, request }));
  }

  private loadTasks(filters: TaskFilters): void {
    this.store.dispatch(
      TasksActions.loadByProject({
        projectId: this.projectId,
        filters,
      })
    );
  }

  private openTaskDialog(data: TaskFormDialogData): void {
    const dialogRef = this.dialog.open(TaskFormModalComponent, {
      width: '560px',
      maxWidth: '95vw',
      autoFocus: 'first-titled-element',
      data,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }

      if (data.task) {
        this.store.dispatch(
          TasksActions.updateTask({
            taskId: data.task.id,
            request: {
              title: result.title,
              description: result.description,
              status: result.status,
              priority: result.priority,
              deadlineUtc: result.deadlineUtc,
            },
          })
        );
      } else {
        this.store.dispatch(
          TasksActions.createTask({
            request: {
              projectId: result.projectId,
              title: result.title,
              description: result.description,
              priority: result.priority,
              deadlineUtc: result.deadlineUtc,
            },
          })
        );
      }
    });
  }
}
