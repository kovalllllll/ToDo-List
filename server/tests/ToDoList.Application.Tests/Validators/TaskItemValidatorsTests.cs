using FluentAssertions;
using ToDoList.Application.Feature.TaskItems.Commands;
using ToDoList.Application.Feature.TaskItems.Validators;
using ToDoList.Domain.Enums;
using Xunit;

namespace ToDoList.Application.Tests.Validators;

public class TaskItemValidatorsTests
{
    [Fact]
    public void CreateTaskItem_ShouldFail_WhenTitleEmpty()
    {
        var validator = new CreateTaskItemCommandValidator();
        var command = new CreateTaskItemCommand(null, string.Empty, null, null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateTaskItem_ShouldFail_WhenTitleTooLong()
    {
        var validator = new CreateTaskItemCommandValidator();
        var command = new CreateTaskItemCommand(null, new string('a', 51), null, null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateTaskItem_ShouldFail_WhenDescriptionTooLong()
    {
        var validator = new CreateTaskItemCommandValidator();
        var command = new CreateTaskItemCommand(null, "Title", new string('a', 201), null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateTaskItem_ShouldFail_WhenProjectIdEmpty()
    {
        var validator = new CreateTaskItemCommandValidator();
        var command = new CreateTaskItemCommand(Guid.Empty, "Title", null, null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateTaskItem_ShouldFail_WhenPriorityInvalid()
    {
        var validator = new CreateTaskItemCommandValidator();
        var command = new CreateTaskItemCommand(null, "Title", null, (TaskPriority)99, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateTaskItem_ShouldFail_WhenDeadlineInPast()
    {
        var validator = new CreateTaskItemCommandValidator();
        var command = new CreateTaskItemCommand(null, "Title", null, null, DateTime.UtcNow.AddMinutes(-5));

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateTaskItem_ShouldPass_WhenValid()
    {
        var validator = new CreateTaskItemCommandValidator();
        var command = new CreateTaskItemCommand(null, "Title", "Desc", TaskPriority.Low,
            DateTime.UtcNow.AddDays(1));

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateTaskItem_ShouldFail_WhenTitleEmpty()
    {
        var validator = new UpdateTaskItemCommandValidator();
        var command = new UpdateTaskItemCommand(
            Guid.NewGuid(),
            string.Empty,
            null,
            TaskItemStatus.Todo,
            null,
            null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateTaskItem_ShouldFail_WhenPriorityInvalid()
    {
        var validator = new UpdateTaskItemCommandValidator();
        var command = new UpdateTaskItemCommand(
            Guid.NewGuid(),
            "Title",
            null,
            TaskItemStatus.Todo,
            (TaskPriority)99,
            null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateTaskItem_ShouldFail_WhenStatusInvalid()
    {
        var validator = new UpdateTaskItemCommandValidator();
        var command = new UpdateTaskItemCommand(
            Guid.NewGuid(),
            "Title",
            null,
            (TaskItemStatus)99,
            null,
            null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateTaskItem_ShouldFail_WhenDeadlineInPast()
    {
        var validator = new UpdateTaskItemCommandValidator();
        var command = new UpdateTaskItemCommand(
            Guid.NewGuid(),
            "Title",
            null,
            TaskItemStatus.Todo,
            null,
            DateTime.UtcNow.AddMinutes(-5));

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateTaskItem_ShouldPass_WhenValid()
    {
        var validator = new UpdateTaskItemCommandValidator();
        var command = new UpdateTaskItemCommand(
            Guid.NewGuid(),
            "Title",
            "Desc",
            TaskItemStatus.Done,
            TaskPriority.High,
            DateTime.UtcNow.AddDays(1));

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeleteTaskItem_ShouldFail_WhenIdEmpty()
    {
        var validator = new DeleteTaskItemCommandValidator();
        var command = new DeleteTaskItemCommand(Guid.Empty);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DeleteTaskItem_ShouldPass_WhenIdValid()
    {
        var validator = new DeleteTaskItemCommandValidator();
        var command = new DeleteTaskItemCommand(Guid.NewGuid());

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
