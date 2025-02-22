using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Repository interface for employees.
/// </summary>
public interface IEmployeeRepository : IBaseRepository<EmployeeEntity>
{
}