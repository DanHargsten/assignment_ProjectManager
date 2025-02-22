using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Repository interface for projects.
/// </summary>
public interface IProjectRepository : IBaseRepository<ProjectEntity>
{
    Task<IEnumerable<ProjectEntity>> GetAllWithCustomerAsync();
}
