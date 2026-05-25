using FluentAssertions;
using ToDoList.Application.Feature.Projects.Commands;
using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Application.Feature.Projects.Queries;
using ToDoList.Application.Feature.Projects.Validators;
using ToDoList.Domain.Enums;
using Xunit;

namespace ToDoList.Application.Tests.Validators;

public class ProjectValidatorsTests
{
    [Fact]
    public void CreateProject_ShouldFail_WhenNameEmpty()
    {
        var validator = new CreateProjectCommandValidator();
        var command = new CreateProjectCommand(string.Empty, null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateProject_ShouldFail_WhenNameTooLong()
    {
        var validator = new CreateProjectCommandValidator();
        var command = new CreateProjectCommand(new string('a', 101), null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateProject_ShouldFail_WhenDescriptionTooLong()
    {
        var validator = new CreateProjectCommandValidator();
        var command = new CreateProjectCommand("Name", new string('a', 501), null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateProject_ShouldFail_WhenColorInvalid()
    {
        var validator = new CreateProjectCommandValidator();
        var command = new CreateProjectCommand("Name", null, "blue");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateProject_ShouldPass_WhenValid()
    {
        var validator = new CreateProjectCommandValidator();
        var command = new CreateProjectCommand("Name", "desc", "#FFF");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateProject_ShouldFail_WhenProjectIdEmpty()
    {
        var validator = new UpdateProjectCommandValidator();
        var command = new UpdateProjectCommand(Guid.Empty, "Name", null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateProject_ShouldFail_WhenNameEmpty()
    {
        var validator = new UpdateProjectCommandValidator();
        var command = new UpdateProjectCommand(Guid.NewGuid(), string.Empty, null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateProject_ShouldFail_WhenColorInvalid()
    {
        var validator = new UpdateProjectCommandValidator();
        var command = new UpdateProjectCommand(Guid.NewGuid(), "Name", null, "123");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateProject_ShouldPass_WhenValid()
    {
        var validator = new UpdateProjectCommandValidator();
        var command = new UpdateProjectCommand(Guid.NewGuid(), "Name", "desc", "#FFFFFF");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeleteProject_ShouldFail_WhenProjectIdEmpty()
    {
        var validator = new DeleteProjectCommandValidator();
        var command = new DeleteProjectCommand(Guid.Empty);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DeleteProject_ShouldPass_WhenProjectIdValid()
    {
        var validator = new DeleteProjectCommandValidator();
        var command = new DeleteProjectCommand(Guid.NewGuid());

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetProjectTasks_ShouldFail_WhenProjectIdEmpty()
    {
        var validator = new GetProjectTasksQueryValidator();
        var query = new GetProjectTasksQuery(
            Guid.Empty,
            new ProjectTasksFilterModel(null, null, null));

        var result = validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetProjectTasks_ShouldFail_WhenSearchTooLong()
    {
        var validator = new GetProjectTasksQueryValidator();
        var query = new GetProjectTasksQuery(
            Guid.NewGuid(),
            new ProjectTasksFilterModel(new string('a', 101), null, null));

        var result = validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetProjectTasks_ShouldFail_WhenStatusesHaveDuplicates()
    {
        var validator = new GetProjectTasksQueryValidator();
        var query = new GetProjectTasksQuery(
            Guid.NewGuid(),
            new ProjectTasksFilterModel(null, [TaskItemStatus.Todo, TaskItemStatus.Todo], null));

        var result = validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetProjectTasks_ShouldFail_WhenPrioritiesHaveDuplicates()
    {
        var validator = new GetProjectTasksQueryValidator();
        var query = new GetProjectTasksQuery(
            Guid.NewGuid(),
            new ProjectTasksFilterModel(null, null, [TaskPriority.Low, TaskPriority.Low]));

        var result = validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetProjectTasks_ShouldPass_WhenValid()
    {
        var validator = new GetProjectTasksQueryValidator();
        var query = new GetProjectTasksQuery(
            Guid.NewGuid(),
            new ProjectTasksFilterModel("text", [TaskItemStatus.Todo], [TaskPriority.Medium]));

        var result = validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }
}
