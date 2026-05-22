using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Users.Models;

namespace ToDoList.Application.Feature.Users.Commands;

public record SignInCommand(string Email, string Password) : IRequest<Result<AuthResponseModel>>;