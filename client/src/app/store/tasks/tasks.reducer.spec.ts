import { tasksReducer, initialTasksState } from './tasks.reducer';
import { TasksActions } from './tasks.actions';
import { TaskItem, TaskItemStatus } from '../../core/models/task.models';

const task: TaskItem = {
  id: '1',
  projectId: 'p1',
  title: 'Task',
  description: null,
  status: TaskItemStatus.Todo,
  priority: null,
  deadlineUtc: null,
};

describe('tasksReducer', () => {
  it('should store tasks on load success', () => {
    const state = tasksReducer(
      initialTasksState,
      TasksActions.loadByProjectSuccess({ tasks: [task] })
    );

    expect(state.items).toEqual([task]);
  });

  it('should update task on update success', () => {
    const loaded = tasksReducer(
      initialTasksState,
      TasksActions.loadByProjectSuccess({ tasks: [task] })
    );

    const updated = { ...task, status: TaskItemStatus.Done };
    const state = tasksReducer(
      loaded,
      TasksActions.updateTaskSuccess({ task: updated })
    );

    expect(state.items[0].status).toBe(TaskItemStatus.Done);
  });
});
