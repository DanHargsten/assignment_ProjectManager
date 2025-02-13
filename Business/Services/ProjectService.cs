using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepository) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;


    // CREATE //
    public async Task<bool> CreateProjectAsync(ProjectRegistrationForm form)
    {
        try
        {
            if (form == null) return false;

            var customerEntity = ProjectFactory.Create(form);
            if (customerEntity == null) return false;

            await _projectRepository.AddAsync(customerEntity!);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateProjectAsync: {ex.Message}");
            return false;
        }
    }


    // READ //
    public async Task<IEnumerable<Project?>> GetProjectsAsync()
    {
        try
        {
            var projectEntities = await _projectRepository.GetAllAsync();
            if (!projectEntities.Any())
                return [];

            return projectEntities
                .Select(ProjectFactory.Create)
                .Where(project => project != null)!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetProjectAsync: {ex.Message}");
            return [];
        }
    }

    public async Task<Project?> GetProjectById(int id)
    {
        try
        {
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            return ProjectFactory.Create(projectEntity!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetProjectByIdAsync: {ex.Message}");
            return null;
        }
    }


    // UPDATE //
    public async Task<bool> UpdateProjectAsync(int id, string title, string? description, string startDate, string? endDate)
    {
        try
        {
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            if (projectEntity == null)
                return false;

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(title))
            {
                projectEntity.Title = title;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                projectEntity.Description = description;
                hasChanges = true;
            }

            // Validate and convert by ChatGPT
            if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParseExact(startDate, "yyyy-mm-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDate))
            {
                projectEntity.StartDate = parsedStartDate;
                hasChanges = true;
            }

            // Validate and convert by ChatGPT
            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParseExact(endDate, "yyyy-mm-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDate))
            {
                projectEntity.EndDate = parsedEndDate;
                hasChanges = true;
            }

            if (!hasChanges) return false;



            var updatedProjectEntity = await _projectRepository.UpdateAsync(projectEntity);
            return updatedProjectEntity != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateCustomerAsync: {ex.Message}");
            return false;
        }
    }


    // DELETE //
    public async Task<bool> RemoveProjectAsync(int id)
    {
        try
        {
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            if (projectEntity == null) return true;

            await _projectRepository.DeleteAsync(projectEntity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RemoveProjectAsync: {ex.Message}");
            return false;
        }
    }
}