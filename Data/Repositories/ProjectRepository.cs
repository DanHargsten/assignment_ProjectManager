using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <summary>
/// Implementation of repository for projects.
/// </summary>
public class ProjectRepository(DataContext context) : BaseRepository<ProjectEntity>(context), IProjectRepository
{
    // ===========================================
    //      GET ALL PROJECTS WITH CUSTOMERS
    // ===========================================
    public async Task<IEnumerable<ProjectEntity>> GetAllWithCustomerAsync()
    {
        return await _dbSet.Include(p => p.Customer).ToListAsync();
    }
}