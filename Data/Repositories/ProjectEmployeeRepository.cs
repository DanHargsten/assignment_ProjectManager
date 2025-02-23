using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <summary>
/// Implementation of a repository for the relationship between projects and employees.
/// </summary>
public class ProjectEmployeeRepository(DataContext context) : BaseRepository<ProjectEmployee>(context), IProjectEmployeeRepository
{
    // ===========================================
    //        REMOVE EMPLOYEE FROM PROJECT
    // ===========================================
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


    // ===========================================
    //        REMOVE ALL EMPLOYEES FROM PROJECT
    // ===========================================
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


    // ===========================================
    //      GET ALL EMPLOYEES BY PROJECT ID
    // ===========================================
    public async Task<IEnumerable<ProjectEmployee>> GetEmployeesByProjectIdAsync(int projectId)
    {
        return await _context.ProjectEmployees
            .Where(pe => pe.ProjectId == projectId)
            .Include(pe => pe.Employee)
            .ToListAsync();
    }



    // ===========================================
    //      GET ALL PROJECTS BY EMPLOYEE ID
    // ===========================================
    public async Task<IEnumerable<Data.Entities.ProjectEntity>> GetProjectsByEmployeeIdAsync(int employeeId)
    {
        return await _context.ProjectEmployees
            .Where(pe => pe.EmployeeId == employeeId)
            .Include(pe => pe.Project)
            .Select(pe => pe.Project!)
            .ToListAsync();
    }
}