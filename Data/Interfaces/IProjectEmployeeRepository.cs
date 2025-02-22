using Data.Entities;

namespace Data.Interfaces;

public interface IProjectEmployeeRepository : IBaseRepository<ProjectEmployee>
{    
    Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId);
    Task<bool> RemoveAllEmployeesFromProjectAsync(int projectId);
    Task<IEnumerable<ProjectEmployee>> GetEmployeesByProjectIdAsync(int projectId);
}