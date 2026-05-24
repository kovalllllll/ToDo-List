using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Repositories;

public interface IProjectRepository
{
    Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken cancellationToken);
    Task CreateAsync(Project project, CancellationToken cancellationToken);
    Task<ProjectResponseModel?> GetByIdAsync(Guid projectId, Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ProjectResponseModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken);

    Task<Guid?> GetSystemProjectIdAsync(Guid userId, CancellationToken cancellationToken);
    
    Task<bool> UpdateAsync(Guid projectId, Guid userId, string name, string? description, string? color,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid projectId, Guid userId, CancellationToken cancellationToken);
}