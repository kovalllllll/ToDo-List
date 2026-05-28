import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateTaskRequest,
  TaskFilters,
  TaskItem,
  UpdateTaskRequest,
} from '../models/task.models';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/tasks`;
  private readonly projectsUrl = `${environment.apiUrl}/projects`;

  getByProjectId(projectId: string, filters: TaskFilters): Observable<TaskItem[]> {
    let params = new HttpParams();

    if (filters.search.trim()) {
      params = params.set('search', filters.search.trim());
    }

    filters.statuses.forEach((status) => {
      params = params.append('statuses', status);
    });

    filters.priorities.forEach((priority) => {
      params = params.append('priorities', priority);
    });

    return this.http.get<TaskItem[]>(`${this.projectsUrl}/${projectId}/tasks`, { params });
  }

  getById(taskId: string): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.baseUrl}/${taskId}`);
  }

  create(request: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.baseUrl, request);
  }

  update(taskId: string, request: UpdateTaskRequest): Observable<TaskItem> {
    return this.http.patch<TaskItem>(`${this.baseUrl}/${taskId}`, request);
  }

  delete(taskId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${taskId}`);
  }
}
