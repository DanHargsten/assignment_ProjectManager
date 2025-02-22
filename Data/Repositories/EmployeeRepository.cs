using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

/// <summary>
/// Implementation of repository for employees.
/// </summary>
public class EmployeeRepository(DataContext context) : BaseRepository<EmployeeEntity>(context), IEmployeeRepository
{
}