using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Feature.TaskItems.Commands;
using ToDoList.Application.Feature.TaskItems.Handlers;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Application.Feature.TaskItems.Queries;
using ToDoList.Application.Repositories;
using ToDoList.Domain.Entities;
using ToDoList.Domain.Enums;
using Xunit;

namespace ToDoList.Application.Tests.Handlers;

public class TaskItemHandlersTests
{
    [Fact]
    public async Task CreateTaskItem_ShouldReturnNotFound_WhenProjectMissing()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var request = new CreateTaskItemCommand(projectId, "Title", "Desc", TaskPriority.Medium, null);
        var logger = new Mock<ILogger<CreateTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ToDoList.Application.Feature.Projects.Models.ProjectResponseModel?)null);

        var handler = new CreateTaskItemCommandHandler(
            logger.Object,
            taskItemRepository.Object,
            currentUserService.Object,
            projectRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task CreateTaskItem_ShouldReturnNotFound_WhenSystemProjectMissing()
    {
        var userId = Guid.NewGuid();
        var request = new CreateTaskItemCommand(null, "Title", "Desc", TaskPriority.Medium, null);
        var logger = new Mock<ILogger<CreateTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetSystemProjectIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        var handler = new CreateTaskItemCommandHandler(
            logger.Object,
            taskItemRepository.Object,
            currentUserService.Object,
            projectRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task CreateTaskItem_ShouldReturnNotFound_WhenCreatedTaskMissing()
    {
        var userId = Guid.NewGuid();
        var systemProjectId = Guid.NewGuid();
        var request = new CreateTaskItemCommand(null, "Title", "Desc", TaskPriority.Medium, null);
        var logger = new Mock<ILogger<CreateTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetSystemProjectIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(systemProjectId);
        taskItemRepository.Setup(x => x.CreateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        taskItemRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItemResponseModel?)null);

        var handler = new CreateTaskItemCommandHandler(
            logger.Object,
            taskItemRepository.Object,
            currentUserService.Object,
            projectRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TaskItemErrors.TaskItemNotFound);
    }

    [Fact]
    public async Task CreateTaskItem_ShouldReturnTask_WhenCreated()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var deadline = DateTime.UtcNow.AddDays(2);
        var request = new CreateTaskItemCommand(projectId, "Title", "Desc", TaskPriority.Medium, deadline);
        var logger = new Mock<ILogger<CreateTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        TaskItem? capturedTask = null;

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ToDoList.Application.Feature.Projects.Models.ProjectResponseModel(
                projectId,
                userId,
                "Project",
                null,
                "#FFFFFF",
                false,
                DateTime.UtcNow,
                []));
        taskItemRepository.Setup(x => x.CreateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((task, _) => capturedTask = task)
            .Returns(Task.CompletedTask);
        taskItemRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, Guid _, CancellationToken _) =>
                new TaskItemResponseModel(
                    id,
                    projectId,
                    request.Title,
                    request.Description,
                    TaskItemStatus.Todo,
                    request.Priority,
                    request.DeadlineUtc));

        var handler = new CreateTaskItemCommandHandler(
            logger.Object,
            taskItemRepository.Object,
            currentUserService.Object,
            projectRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be(request.Title);
        result.Value.Description.Should().Be(request.Description);
        result.Value.ProjectId.Should().Be(projectId);
        result.Value.Priority.Should().Be(request.Priority);
        result.Value.DeadlineUtc.Should().Be(deadline);

        capturedTask.Should().NotBeNull();
        capturedTask!.ProjectId.Should().Be(projectId);
        capturedTask.Title.Should().Be(request.Title);
        capturedTask.Description.Should().Be(request.Description);
        capturedTask.Priority.Should().Be(request.Priority);
        capturedTask.DeadlineUtc.Should().Be(request.DeadlineUtc);
    }

    [Fact]
    public async Task UpdateTaskItem_ShouldReturnNotFound_WhenTaskMissing()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateTaskItemCommand(
            Guid.NewGuid(),
            "Title",
            null,
            TaskItemStatus.Done,
            TaskPriority.High,
            null);
        var logger = new Mock<ILogger<UpdateTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        taskItemRepository.Setup(x => x.GetByIdAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItemResponseModel?)null);

        var handler = new UpdateTaskItemCommandHandler(
            logger.Object,
            currentUserService.Object,
            taskItemRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TaskItemErrors.TaskItemNotFound);
    }

    [Fact]
    public async Task UpdateTaskItem_ShouldReturnConflict_WhenUpdateFails()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateTaskItemCommand(
            Guid.NewGuid(),
            "Title",
            null,
            TaskItemStatus.Done,
            TaskPriority.High,
            null);
        var logger = new Mock<ILogger<UpdateTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var existing = new TaskItemResponseModel(
            request.Id,
            Guid.NewGuid(),
            "Old",
            null,
            TaskItemStatus.Todo,
            TaskPriority.Low,
            null);

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        taskItemRepository.SetupSequence(x => x.GetByIdAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing)
            .ReturnsAsync((TaskItemResponseModel?)null);
        taskItemRepository.Setup(x => x.UpdateAsync(
                request.Id,
                userId,
                request.Title,
                request.Description,
                request.Status,
                request.Priority,
                request.DeadlineUtc,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdateTaskItemCommandHandler(
            logger.Object,
            currentUserService.Object,
            taskItemRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TaskItemErrors.TaskItemUpdateFailed);
    }

    [Fact]
    public async Task UpdateTaskItem_ShouldReturnNotFound_WhenUpdatedTaskMissing()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateTaskItemCommand(
            Guid.NewGuid(),
            "Title",
            null,
            TaskItemStatus.Done,
            TaskPriority.High,
            null);
        var logger = new Mock<ILogger<UpdateTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var existing = new TaskItemResponseModel(
            request.Id,
            Guid.NewGuid(),
            "Old",
            null,
            TaskItemStatus.Todo,
            TaskPriority.Low,
            null);

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        taskItemRepository.SetupSequence(x => x.GetByIdAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing)
            .ReturnsAsync((TaskItemResponseModel?)null);
        taskItemRepository.Setup(x => x.UpdateAsync(
                request.Id,
                userId,
                request.Title,
                request.Description,
                request.Status,
                request.Priority,
                request.DeadlineUtc,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var handler = new UpdateTaskItemCommandHandler(
            logger.Object,
            currentUserService.Object,
            taskItemRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TaskItemErrors.TaskItemNotFound);
    }

    [Fact]
    public async Task UpdateTaskItem_ShouldReturnUpdatedTask_WhenSuccessful()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateTaskItemCommand(
            Guid.NewGuid(),
            "Title",
            "Desc",
            TaskItemStatus.Done,
            TaskPriority.High,
            DateTime.UtcNow.AddDays(1));
        var logger = new Mock<ILogger<UpdateTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var existing = new TaskItemResponseModel(
            request.Id,
            Guid.NewGuid(),
            "Old",
            null,
            TaskItemStatus.Todo,
            TaskPriority.Low,
            null);
        var updatedTask = new TaskItemResponseModel(
            request.Id,
            existing.ProjectId,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DeadlineUtc);

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        taskItemRepository.Setup(x => x.GetByIdAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        taskItemRepository.Setup(x => x.UpdateAsync(
                request.Id,
                userId,
                request.Title,
                request.Description,
                request.Status,
                request.Priority,
                request.DeadlineUtc,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        taskItemRepository.Setup(x => x.GetByIdAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTask);

        var handler = new UpdateTaskItemCommandHandler(
            logger.Object,
            currentUserService.Object,
            taskItemRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(updatedTask);
    }

    [Fact]
    public async Task DeleteTaskItem_ShouldReturnNotFound_WhenDeleteFails()
    {
        var userId = Guid.NewGuid();
        var request = new DeleteTaskItemCommand(Guid.NewGuid());
        var logger = new Mock<ILogger<DeleteTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        taskItemRepository.Setup(x => x.DeleteAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new DeleteTaskItemCommandHandler(
            logger.Object,
            currentUserService.Object,
            taskItemRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TaskItemErrors.TaskItemNotFound);
    }

    [Fact]
    public async Task DeleteTaskItem_ShouldReturnSuccess_WhenDeleted()
    {
        var userId = Guid.NewGuid();
        var request = new DeleteTaskItemCommand(Guid.NewGuid());
        var logger = new Mock<ILogger<DeleteTaskItemCommandHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        taskItemRepository.Setup(x => x.DeleteAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteTaskItemCommandHandler(
            logger.Object,
            currentUserService.Object,
            taskItemRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetTaskItemById_ShouldReturnNotFound_WhenMissing()
    {
        var userId = Guid.NewGuid();
        var request = new GetTaskItemByIdQuery(Guid.NewGuid());
        var logger = new Mock<ILogger<GetTaskItemByIdQueryHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        taskItemRepository.Setup(x => x.GetByIdAsync(request.TaskId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItemResponseModel?)null);

        var handler = new GetTaskItemByIdQueryHandler(
            logger.Object,
            taskItemRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TaskItemErrors.TaskItemNotFound);
    }

    [Fact]
    public async Task GetTaskItemById_ShouldReturnTask_WhenFound()
    {
        var userId = Guid.NewGuid();
        var request = new GetTaskItemByIdQuery(Guid.NewGuid());
        var logger = new Mock<ILogger<GetTaskItemByIdQueryHandler>>();
        var taskItemRepository = new Mock<ITaskItemRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var task = new TaskItemResponseModel(
            request.TaskId,
            Guid.NewGuid(),
            "Title",
            null,
            TaskItemStatus.InProgress,
            TaskPriority.Medium,
            null);

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        taskItemRepository.Setup(x => x.GetByIdAsync(request.TaskId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var handler = new GetTaskItemByIdQueryHandler(
            logger.Object,
            taskItemRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(task);
    }
}
