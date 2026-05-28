import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Store } from '@ngrx/store';
import { Actions, ofType } from '@ngrx/effects';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Project } from '../../../core/models/project.models';
import { ProjectsActions } from '../../../store/projects/projects.actions';
import {
  selectAllProjects,
  selectProjectsError,
  selectProjectsLoading,
} from '../../../store/projects/projects.selectors';
import { ProjectCardComponent } from '../project-card/project-card.component';
import {
  ProjectFormDialogData,
  ProjectFormModalComponent,
} from '../project-form-modal/project-form-modal.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    ProjectCardComponent,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly actions$ = inject(Actions);

  readonly projects = this.store.selectSignal(selectAllProjects);
  readonly loading = this.store.selectSignal(selectProjectsLoading);
  readonly error = this.store.selectSignal(selectProjectsError);

  constructor() {
    this.actions$
      .pipe(
        ofType(
          ProjectsActions.createProjectFailure,
          ProjectsActions.updateProjectFailure,
          ProjectsActions.deleteProjectFailure
        ),
        takeUntilDestroyed()
      )
      .subscribe(({ error }) => this.showError(error));

    this.actions$
      .pipe(
        ofType(
          ProjectsActions.createProjectSuccess,
          ProjectsActions.updateProjectSuccess,
          ProjectsActions.deleteProjectSuccess
        ),
        takeUntilDestroyed()
      )
      .subscribe(() => this.snackBar.open('Project updated', 'Close', { duration: 3000 }));
  }

  ngOnInit(): void {
    this.store.dispatch(ProjectsActions.loadAll());
  }

  openCreateDialog(): void {
    this.openProjectDialog();
  }

  openEditDialog(project: Project): void {
    this.openProjectDialog({ project });
  }

  deleteProject(project: Project): void {
    if (project.isSystem) {
      return;
    }

    if (confirm(`Delete project "${project.name}"?`)) {
      this.store.dispatch(ProjectsActions.deleteProject({ projectId: project.id }));
    }
  }

  private openProjectDialog(data: ProjectFormDialogData = {}): void {
    const dialogRef = this.dialog.open(ProjectFormModalComponent, {
      width: '480px',
      maxWidth: '95vw',
      autoFocus: 'first-titled-element',
      data,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }

      if (data.project) {
        this.store.dispatch(
          ProjectsActions.updateProject({
            projectId: data.project.id,
            request: result,
          })
        );
      } else {
        this.store.dispatch(ProjectsActions.createProject({ request: result }));
      }
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', { duration: 5000 });
  }
}
