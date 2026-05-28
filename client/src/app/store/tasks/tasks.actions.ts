import { createActionGroup, emptyProps, props } from '@ngrx/store';
import {
  CreateTaskRequest,
  TaskFilters,
  TaskItem,
  UpdateTaskRequest,
} from '../../core/models/task.models';

export const TasksActions = createActionGroup({
  source: 'Tasks',
  events: {
    'Load By Project': props<{ projectId: string; filters: TaskFilters }>(),
    'Load By Project Success': props<{ tasks: TaskItem[] }>(),
    'Load By Project Failure': props<{ error: string }>(),
    'Set Filters': props<{ filters: TaskFilters }>(),
    'Create Task': props<{ request: CreateTaskRequest }>(),
    'Create Task Success': props<{ task: TaskItem }>(),
    'Create Task Failure': props<{ error: string }>(),
    'Update Task': props<{ taskId: string; request: UpdateTaskRequest }>(),
    'Update Task Success': props<{ task: TaskItem }>(),
    'Update Task Failure': props<{ error: string }>(),
    'Delete Task': props<{ taskId: string }>(),
    'Delete Task Success': props<{ taskId: string }>(),
    'Delete Task Failure': props<{ error: string }>(),
    'Clear Tasks': emptyProps(),
  },
});
