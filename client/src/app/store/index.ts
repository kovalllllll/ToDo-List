import { EnvironmentProviders, isDevMode } from '@angular/core';
import { provideEffects } from '@ngrx/effects';
import { provideState, provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { authReducer } from './auth/auth.reducer';
import { AuthEffects } from './auth/auth.effects';
import { projectsReducer } from './projects/projects.reducer';
import { ProjectsEffects } from './projects/projects.effects';
import { tasksReducer } from './tasks/tasks.reducer';
import { TasksEffects } from './tasks/tasks.effects';

export const storeProviders: EnvironmentProviders[] = [
  provideStore(),
  provideState('auth', authReducer),
  provideState('projects', projectsReducer),
  provideState('tasks', tasksReducer),
  provideEffects(AuthEffects, ProjectsEffects, TasksEffects),
  ...(isDevMode() ? [provideStoreDevtools({ maxAge: 25 })] : []),
];
