import { TaskItem } from './task.models';

export interface Project {
  id: string;
  userId: string;
  name: string;
  description: string | null;
  color: string | null;
  isSystem: boolean;
  createdAt: string;
  tasks: TaskItem[];
}

export interface CreateProjectRequest {
  name: string;
  description?: string | null;
  color?: string | null;
}

export interface UpdateProjectRequest {
  name: string;
  description?: string | null;
  color?: string | null;
}
