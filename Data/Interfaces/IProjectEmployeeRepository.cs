using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Repository interface for the relationship between projects and employees.
/// </summary>
public interface IProjectEmployeeRepository : IBaseRepository<ProjectEmployee>
{    
    Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId);
    Task<bool> RemoveAllEmployeesFromProjectAsync(int projectId);
    Task<IEnumerable<ProjectEmployee>> GetEmployeesByProjectIdAsync(int projectId);
}