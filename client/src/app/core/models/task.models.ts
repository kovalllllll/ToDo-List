export enum TaskItemStatus {
  Todo = 'Todo',
  InProgress = 'InProgress',
  Done = 'Done',
}

export enum TaskPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
}

export interface TaskItem {
  id: string;
  projectId: string;
  title: string;
  description: string | null;
  status: TaskItemStatus;
  priority: TaskPriority | null;
  deadlineUtc: string | null;
}

export interface CreateTaskRequest {
  projectId?: string | null;
  title: string;
  description?: string | null;
  priority?: TaskPriority | null;
  deadlineUtc?: string | null;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string | null;
  status: TaskItemStatus;
  priority?: TaskPriority | null;
  deadlineUtc?: string | null;
}

export interface TaskFilters {
  search: string;
  statuses: TaskItemStatus[];
  priorities: TaskPriority[];
}

export const DEFAULT_TASK_FILTERS: TaskFilters = {
  search: '',
  statuses: [],
  priorities: [],
};

export const TASK_STATUS_LABELS: Record<TaskItemStatus, string> = {
  [TaskItemStatus.Todo]: 'To Do',
  [TaskItemStatus.InProgress]: 'In Progress',
  [TaskItemStatus.Done]: 'Done',
};

export const TASK_PRIORITY_LABELS: Record<TaskPriority, string> = {
  [TaskPriority.Low]: 'Low',
  [TaskPriority.Medium]: 'Medium',
  [TaskPriority.High]: 'High',
};

export const ALL_TASK_STATUSES = Object.values(TaskItemStatus);
export const ALL_TASK_PRIORITIES = Object.values(TaskPriority);
