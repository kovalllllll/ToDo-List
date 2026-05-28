import { createActionGroup, emptyProps, props } from '@ngrx/store';
import {
  CreateProjectRequest,
  Project,
  UpdateProjectRequest,
} from '../../core/models/project.models';

export const ProjectsActions = createActionGroup({
  source: 'Projects',
  events: {
    'Load All': emptyProps(),
    'Load All Success': props<{ projects: Project[] }>(),
    'Load All Failure': props<{ error: string }>(),
    'Load By Id': props<{ projectId: string }>(),
    'Load By Id Success': props<{ project: Project }>(),
    'Load By Id Failure': props<{ error: string }>(),
    'Create Project': props<{ request: CreateProjectRequest }>(),
    'Create Project Success': props<{ project: Project }>(),
    'Create Project Failure': props<{ error: string }>(),
    'Update Project': props<{ projectId: string; request: UpdateProjectRequest }>(),
    'Update Project Success': props<{ project: Project }>(),
    'Update Project Failure': props<{ error: string }>(),
    'Delete Project': props<{ projectId: string }>(),
    'Delete Project Success': props<{ projectId: string }>(),
    'Delete Project Failure': props<{ error: string }>(),
    'Clear Selected Project': emptyProps(),
  },
});
