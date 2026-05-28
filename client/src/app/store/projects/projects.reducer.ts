import { Project } from '../../core/models/project.models';
import { ProjectsActions } from './projects.actions';
import { createReducer, on } from '@ngrx/store';

export interface ProjectsState {
  items: Project[];
  selectedProject: Project | null;
  loading: boolean;
  error: string | null;
}

export const initialProjectsState: ProjectsState = {
  items: [],
  selectedProject: null,
  loading: false,
  error: null,
};

export const projectsReducer = createReducer(
  initialProjectsState,
  on(
    ProjectsActions.loadAll,
    ProjectsActions.loadById,
    ProjectsActions.createProject,
    ProjectsActions.updateProject,
    ProjectsActions.deleteProject,
    (state) => ({
      ...state,
      loading: true,
      error: null,
    })
  ),
  on(ProjectsActions.loadAllSuccess, (state, { projects }) => ({
    ...state,
    items: projects,
    loading: false,
  })),
  on(ProjectsActions.loadByIdSuccess, (state, { project }) => ({
    ...state,
    selectedProject: project,
    loading: false,
  })),
  on(ProjectsActions.createProjectSuccess, (state, { project }) => ({
    ...state,
    items: [...state.items, project],
    loading: false,
  })),
  on(ProjectsActions.updateProjectSuccess, (state, { project }) => ({
    ...state,
    items: state.items.map((item) => (item.id === project.id ? project : item)),
    selectedProject: state.selectedProject?.id === project.id ? project : state.selectedProject,
    loading: false,
  })),
  on(ProjectsActions.deleteProjectSuccess, (state, { projectId }) => ({
    ...state,
    items: state.items.filter((item) => item.id !== projectId),
    selectedProject: state.selectedProject?.id === projectId ? null : state.selectedProject,
    loading: false,
  })),
  on(
    ProjectsActions.loadAllFailure,
    ProjectsActions.loadByIdFailure,
    ProjectsActions.createProjectFailure,
    ProjectsActions.updateProjectFailure,
    ProjectsActions.deleteProjectFailure,
    (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })
  ),
  on(ProjectsActions.clearSelectedProject, (state) => ({
    ...state,
    selectedProject: null,
  }))
);
