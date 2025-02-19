using Business.Models;
using Data.Enums;

namespace Business.Interfaces
{
    public interface IProjectService
    {
        Task<bool> CreateProjectAsync(ProjectRegistrationForm form);
        Task<Project?> GetProjectByIdAsync(int id);
        Task<IEnumerable<Project?>> GetProjectsAsync();
        Task<bool> RemoveProjectAsync(int id);
        Task<bool> UpdateProjectAsync(int id, string title, string? description, DateTime? startDate, DateTime? endDate, ProjectStatus status);
        Task<IEnumerable<Project>> GetProjectsByCustomerIdAsync(int customerId);
        Task<IEnumerable<Project>> GetProjectsByCustomerNameOrEmailAsync(string searchTerm);
    }
}