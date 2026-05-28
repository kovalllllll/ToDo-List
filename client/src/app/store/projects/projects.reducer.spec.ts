import { projectsReducer, initialProjectsState } from './projects.reducer';
import { ProjectsActions } from './projects.actions';
import { Project } from '../../core/models/project.models';

const project: Project = {
  id: '1',
  userId: 'u1',
  name: 'Demo',
  description: null,
  color: '#fff',
  isSystem: false,
  createdAt: '2026-01-01T00:00:00Z',
  tasks: [],
};

describe('projectsReducer', () => {
  it('should store projects on load success', () => {
    const state = projectsReducer(
      initialProjectsState,
      ProjectsActions.loadAllSuccess({ projects: [project] })
    );

    expect(state.items).toEqual([project]);
    expect(state.loading).toBe(false);
  });

  it('should remove project on delete success', () => {
    const loaded = projectsReducer(
      initialProjectsState,
      ProjectsActions.loadAllSuccess({ projects: [project] })
    );

    const state = projectsReducer(
      loaded,
      ProjectsActions.deleteProjectSuccess({ projectId: project.id })
    );

    expect(state.items).toEqual([]);
  });
});
