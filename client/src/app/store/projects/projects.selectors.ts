import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ProjectsState } from './projects.reducer';

export const selectProjectsState = createFeatureSelector<ProjectsState>('projects');

export const selectAllProjects = createSelector(selectProjectsState, (state) => state.items);
export const selectSelectedProject = createSelector(
  selectProjectsState,
  (state) => state.selectedProject
);
export const selectProjectsLoading = createSelector(selectProjectsState, (state) => state.loading);
export const selectProjectsError = createSelector(selectProjectsState, (state) => state.error);
