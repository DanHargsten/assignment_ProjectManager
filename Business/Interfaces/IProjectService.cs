using Business.Models;

namespace Business.Interfaces
{
    public interface IProjectService
    {
        Task<bool> CreateProjectAsync(ProjectRegistrationForm form);
        Task<Project?> GetProjectById(int id);
        Task<IEnumerable<Project?>> GetProjectsAsync();
        Task<bool> RemoveProjectAsync(int id);
        Task<bool> UpdateProjectAsync(int id, string title, string? description, string startDate, string? endDate);
        Task<IEnumerable<Project>> GetProjectsByCustomerIdAsync(int customerId);
    }
}