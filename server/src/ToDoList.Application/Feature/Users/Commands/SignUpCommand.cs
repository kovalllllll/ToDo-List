using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Users.Models;

namespace ToDoList.Application.Feature.Users.Commands;

public record SignUpCommand(string Email, string UserName, string Password) : IRequest<Result<AuthResponseModel>>;