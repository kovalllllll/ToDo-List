import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';
import { getErrorMessage } from '../../core/models/api-error.models';
import { ProjectService } from '../../core/services/project.service';
import { ProjectsActions } from './projects.actions';

@Injectable()
export class ProjectsEffects {
  private readonly actions$ = inject(Actions);
  private readonly projectService = inject(ProjectService);

  loadAll$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectsActions.loadAll),
      switchMap(() =>
        this.projectService.getAll().pipe(
          map((projects) => ProjectsActions.loadAllSuccess({ projects })),
          catchError((error) =>
            of(ProjectsActions.loadAllFailure({ error: getErrorMessage(error) }))
          )
        )
      )
    )
  );

  loadById$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectsActions.loadById),
      switchMap(({ projectId }) =>
        this.projectService.getById(projectId).pipe(
          map((project) => ProjectsActions.loadByIdSuccess({ project })),
          catchError((error) =>
            of(ProjectsActions.loadByIdFailure({ error: getErrorMessage(error) }))
          )
        )
      )
    )
  );

  create$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectsActions.createProject),
      switchMap(({ request }) =>
        this.projectService.create(request).pipe(
          map((project) => ProjectsActions.createProjectSuccess({ project })),
          catchError((error) =>
            of(ProjectsActions.createProjectFailure({ error: getErrorMessage(error) }))
          )
        )
      )
    )
  );

  update$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectsActions.updateProject),
      switchMap(({ projectId, request }) =>
        this.projectService.update(projectId, request).pipe(
          map((project) => ProjectsActions.updateProjectSuccess({ project })),
          catchError((error) =>
            of(ProjectsActions.updateProjectFailure({ error: getErrorMessage(error) }))
          )
        )
      )
    )
  );

  delete$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectsActions.deleteProject),
      switchMap(({ projectId }) =>
        this.projectService.delete(projectId).pipe(
          map(() => ProjectsActions.deleteProjectSuccess({ projectId })),
          catchError((error) =>
            of(ProjectsActions.deleteProjectFailure({ error: getErrorMessage(error) }))
          )
        )
      )
    )
  );
}
