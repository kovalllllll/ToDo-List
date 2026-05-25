using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Feature.Projects.Commands;
using ToDoList.Application.Feature.Projects.Handlers;
using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Application.Feature.Projects.Queries;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Application.Repositories;
using ToDoList.Domain.Entities;
using ToDoList.Domain.Enums;
using Xunit;

namespace ToDoList.Application.Tests.Handlers;

public class ProjectHandlersTests
{
    [Fact]
    public async Task CreateProject_ShouldReturnConflict_WhenNameExists()
    {
        var userId = Guid.NewGuid();
        var request = new CreateProjectCommand("Inbox", "desc", "#FFFFFF");
        var logger = new Mock<ILogger<CreateProjectCommandHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.ExistsByNameAsync(userId, request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateProjectCommandHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectAlreadyExists);
        projectRepository.Verify(x => x.CreateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateProject_ShouldReturnProject_WhenCreated()
    {
        var userId = Guid.NewGuid();
        var request = new CreateProjectCommand("Work", "desc", "#ABC");
        var logger = new Mock<ILogger<CreateProjectCommandHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        Project? capturedProject = null;

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.ExistsByNameAsync(userId, request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        projectRepository.Setup(x => x.CreateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Callback<Project, CancellationToken>((project, _) => capturedProject = project)
            .Returns(Task.CompletedTask);

        var handler = new CreateProjectCommandHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);
        result.Value.Color.Should().Be(request.Color);
        result.Value.IsSystem.Should().BeFalse();
        result.Value.Tasks.Should().BeEmpty();
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

        capturedProject.Should().NotBeNull();
        capturedProject!.UserId.Should().Be(userId);
        capturedProject.Name.Should().Be(request.Name);
        capturedProject.Description.Should().Be(request.Description);
        capturedProject.Color.Should().Be(request.Color);
        capturedProject.IsSystem.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateProject_ShouldReturnNotFound_WhenUpdateFails()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateProjectCommand(Guid.NewGuid(), "Updated", null, null);
        var logger = new Mock<ILogger<UpdateProjectCommandHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.UpdateAsync(
                request.ProjectId,
                userId,
                request.Name,
                request.Description,
                request.Color,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdateProjectCommandHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task UpdateProject_ShouldReturnNotFound_WhenProjectMissingAfterUpdate()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateProjectCommand(Guid.NewGuid(), "Updated", null, null);
        var logger = new Mock<ILogger<UpdateProjectCommandHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.UpdateAsync(
                request.ProjectId,
                userId,
                request.Name,
                request.Description,
                request.Color,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        projectRepository.Setup(x => x.GetByIdAsync(request.ProjectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectResponseModel?)null);

        var handler = new UpdateProjectCommandHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task UpdateProject_ShouldReturnUpdatedProject_WhenSuccessful()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateProjectCommand(Guid.NewGuid(), "Updated", "desc", "#123456");
        var logger = new Mock<ILogger<UpdateProjectCommandHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var response = new ProjectResponseModel(
            request.ProjectId,
            userId,
            request.Name,
            request.Description,
            request.Color,
            false,
            DateTime.UtcNow,
            []);

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.UpdateAsync(
                request.ProjectId,
                userId,
                request.Name,
                request.Description,
                request.Color,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        projectRepository.Setup(x => x.GetByIdAsync(request.ProjectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var handler = new UpdateProjectCommandHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(response);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnNotFound_WhenProjectMissing()
    {
        var userId = Guid.NewGuid();
        var request = new DeleteProjectCommand(Guid.NewGuid());
        var logger = new Mock<ILogger<DeleteProjectCommandHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetByIdAsync(request.ProjectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectResponseModel?)null);

        var handler = new DeleteProjectCommandHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnConflict_WhenSystemProject()
    {
        var userId = Guid.NewGuid();
        var request = new DeleteProjectCommand(Guid.NewGuid());
        var logger = new Mock<ILogger<DeleteProjectCommandHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var project = new ProjectResponseModel(
            request.ProjectId,
            userId,
            "Inbox",
            null,
            "#FFFFFF",
            true,
            DateTime.UtcNow,
            []);

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetByIdAsync(request.ProjectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var handler = new DeleteProjectCommandHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.SystemProjectCannotBeDeleted);
        projectRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnNotFound_WhenDeleteFails()
    {
        var userId = Guid.NewGuid();
        var request = new DeleteProjectCommand(Guid.NewGuid());
        var logger = new Mock<ILogger<DeleteProjectCommandHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var project = new ProjectResponseModel(
            request.ProjectId,
            userId,
            "Project",
            null,
            "#FFFFFF",
            false,
            DateTime.UtcNow,
            []);

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetByIdAsync(request.ProjectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        projectRepository.Setup(x => x.DeleteAsync(request.ProjectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new DeleteProjectCommandHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnSuccess_WhenDeleted()
    {
        var userId = Guid.NewGuid();
        var request = new DeleteProjectCommand(Guid.NewGuid());
        var logger = new Mock<ILogger<DeleteProjectCommandHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var project = new ProjectResponseModel(
            request.ProjectId,
            userId,
            "Project",
            null,
            "#FFFFFF",
            false,
            DateTime.UtcNow,
            []);

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetByIdAsync(request.ProjectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        projectRepository.Setup(x => x.DeleteAsync(request.ProjectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteProjectCommandHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetProjectById_ShouldReturnNotFound_WhenMissing()
    {
        var userId = Guid.NewGuid();
        var request = new GetProjectByIdQuery(Guid.NewGuid());
        var logger = new Mock<ILogger<GetProjectByIdQueryHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetByIdAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectResponseModel?)null);

        var handler = new GetProjectByIdQueryHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task GetProjectById_ShouldReturnProject_WhenFound()
    {
        var userId = Guid.NewGuid();
        var request = new GetProjectByIdQuery(Guid.NewGuid());
        var logger = new Mock<ILogger<GetProjectByIdQueryHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var project = new ProjectResponseModel(
            request.Id,
            userId,
            "Project",
            null,
            "#FFFFFF",
            false,
            DateTime.UtcNow,
            []);

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetByIdAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var handler = new GetProjectByIdQueryHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(project);
    }

    [Fact]
    public async Task GetProjectTasks_ShouldReturnTasks()
    {
        var userId = Guid.NewGuid();
        var request = new GetProjectTasksQuery(
            Guid.NewGuid(),
            new ProjectTasksFilterModel("task", [TaskItemStatus.Todo], [TaskPriority.Medium]));
        var logger = new Mock<ILogger<GetProjectTasksQueryHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var tasks = new[]
        {
            new TaskItemResponseModel(
                Guid.NewGuid(),
                request.ProjectId,
                "Title",
                null,
                TaskItemStatus.Todo,
                TaskPriority.Medium,
                null)
        };

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetAllTasksByProjectIdAsync(
                request.ProjectId,
                userId,
                request.Filter,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        var handler = new GetProjectTasksQueryHandler(
            logger.Object,
            currentUserService.Object,
            projectRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(tasks);
    }

    [Fact]
    public async Task ListProjectsByCurrentUser_ShouldReturnProjects()
    {
        var userId = Guid.NewGuid();
        var request = new ListProjectsByCurrentUserQuery();
        var logger = new Mock<ILogger<ListProjectsByCurrentUserQueryHandler>>();
        var projectRepository = new Mock<IProjectRepository>();
        var currentUserService = new Mock<ICurrentUserService>();
        var projects = new[]
        {
            new ProjectResponseModel(
                Guid.NewGuid(),
                userId,
                "Project",
                null,
                "#FFFFFF",
                false,
                DateTime.UtcNow,
                [])
        };

        currentUserService.SetupGet(x => x.UserId).Returns(userId);
        projectRepository.Setup(x => x.GetAllAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        var handler = new ListProjectsByCurrentUserQueryHandler(
            logger.Object,
            projectRepository.Object,
            currentUserService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(projects);
    }
}
