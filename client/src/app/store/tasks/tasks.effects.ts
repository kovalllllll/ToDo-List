import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';
import { getErrorMessage } from '../../core/models/api-error.models';
import { TaskService } from '../../core/services/task.service';
import { TasksActions } from './tasks.actions';

@Injectable()
export class TasksEffects {
  private readonly actions$ = inject(Actions);
  private readonly taskService = inject(TaskService);

  loadByProject$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TasksActions.loadByProject),
      switchMap(({ projectId, filters }) =>
        this.taskService.getByProjectId(projectId, filters).pipe(
          map((tasks) => TasksActions.loadByProjectSuccess({ tasks })),
          catchError((error) =>
            of(TasksActions.loadByProjectFailure({ error: getErrorMessage(error) }))
          )
        )
      )
    )
  );

  create$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TasksActions.createTask),
      switchMap(({ request }) =>
        this.taskService.create(request).pipe(
          map((task) => TasksActions.createTaskSuccess({ task })),
          catchError((error) =>
            of(TasksActions.createTaskFailure({ error: getErrorMessage(error) }))
          )
        )
      )
    )
  );

  update$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TasksActions.updateTask),
      switchMap(({ taskId, request }) =>
        this.taskService.update(taskId, request).pipe(
          map((task) => TasksActions.updateTaskSuccess({ task })),
          catchError((error) =>
            of(TasksActions.updateTaskFailure({ error: getErrorMessage(error) }))
          )
        )
      )
    )
  );

  delete$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TasksActions.deleteTask),
      switchMap(({ taskId }) =>
        this.taskService.delete(taskId).pipe(
          map(() => TasksActions.deleteTaskSuccess({ taskId })),
          catchError((error) =>
            of(TasksActions.deleteTaskFailure({ error: getErrorMessage(error) }))
          )
        )
      )
    )
  );
}
