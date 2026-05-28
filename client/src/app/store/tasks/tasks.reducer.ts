import { DEFAULT_TASK_FILTERS, TaskFilters, TaskItem } from '../../core/models/task.models';
import { TasksActions } from './tasks.actions';
import { createReducer, on } from '@ngrx/store';

export interface TasksState {
  items: TaskItem[];
  filters: TaskFilters;
  loading: boolean;
  error: string | null;
}

export const initialTasksState: TasksState = {
  items: [],
  filters: DEFAULT_TASK_FILTERS,
  loading: false,
  error: null,
};

export const tasksReducer = createReducer(
  initialTasksState,
  on(TasksActions.loadByProject, TasksActions.createTask, TasksActions.updateTask, TasksActions.deleteTask, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(TasksActions.loadByProjectSuccess, (state, { tasks }) => ({
    ...state,
    items: tasks,
    loading: false,
  })),
  on(TasksActions.setFilters, (state, { filters }) => ({
    ...state,
    filters,
  })),
  on(TasksActions.createTaskSuccess, (state, { task }) => ({
    ...state,
    items: [...state.items, task],
    loading: false,
  })),
  on(TasksActions.updateTaskSuccess, (state, { task }) => ({
    ...state,
    items: state.items.map((item) => (item.id === task.id ? task : item)),
    loading: false,
  })),
  on(TasksActions.deleteTaskSuccess, (state, { taskId }) => ({
    ...state,
    items: state.items.filter((item) => item.id !== taskId),
    loading: false,
  })),
  on(
    TasksActions.loadByProjectFailure,
    TasksActions.createTaskFailure,
    TasksActions.updateTaskFailure,
    TasksActions.deleteTaskFailure,
    (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })
  ),
  on(TasksActions.clearTasks, () => initialTasksState)
);
