import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.authRoutes),
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'projects',
        loadChildren: () =>
          import('./features/projects/projects.routes').then((m) => m.projectsRoutes),
      },
      {
        path: 'projects',
        loadChildren: () => import('./features/tasks/tasks.routes').then((m) => m.tasksRoutes),
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'projects',
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'projects',
  },
];
