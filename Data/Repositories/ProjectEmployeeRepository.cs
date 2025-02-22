using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ProjectEmployeeRepository(DataContext context) : BaseRepository<ProjectEmployee>(context), IProjectEmployeeRepository
{
    /// <summary>
    /// Removes an employee from a project.
    /// </summary>
    public async Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId)
    {
        var projectEmployee = await _context.ProjectEmployees
            .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

        if (projectEmployee == null)
            return false;

        _context.ProjectEmployees.Remove(projectEmployee);
        await _context.SaveChangesAsync();
        return true;
    }




    /// <summary>
    /// Removes all employees assigned to a specific project.
    /// </summary>
    public async Task<bool> RemoveAllEmployeesFromProjectAsync(int projectId)
    {
        var entries = await _context.ProjectEmployees
            .Where(pe => pe.ProjectId == projectId)
            .ToListAsync();

        if (entries.Count == 0)
            return false;

        _context.ProjectEmployees.RemoveRange(entries);
        await _context.SaveChangesAsync();
        return true;
    }




    /// <summary>
    /// Retrieves all employees assigned to a specific project.
    /// </summary>
    public async Task<IEnumerable<ProjectEmployee>> GetEmployeesByProjectIdAsync(int projectId)
    {
        return await _context.ProjectEmployees
            .Where(pe => pe.ProjectId == projectId)
            .Include(pe => pe.Employee)
            .ToListAsync();
    }
}