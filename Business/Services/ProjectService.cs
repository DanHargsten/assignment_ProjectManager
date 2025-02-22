using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Enums;
using Data.Interfaces;

namespace Business.Services;



/// <summary>
/// Provides functionality for managing projects, including creation, retrieval, updates, and employee assignments.
/// </summary>
public class ProjectService(
    IProjectRepository projectRepository,
    IEmployeeRepository employeeRepository,
    ICustomerRepository customerRepository,
    IProjectEmployeeRepository projectEmployeeRepository
    ) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IProjectEmployeeRepository _projectEmployeeRepository = projectEmployeeRepository;




    // ==================================================
    //                  CREATE PROJECT
    // ==================================================

    /// <summary>
    /// Creates a new project and stores it in the database.
    /// </summary>
    /// <param name="form">The project registration form containing project details.</param>
    /// <returns>Returns true if the project was created successfully, otherwise false.</returns>
    public async Task<bool> CreateProjectAsync(ProjectRegistrationForm form)
    {
        try
        {
            // Kontrollera att kunden finns innan projektet skapas
            var customer = await _customerRepository.GetOneAsync(x => x.Id == form.CustomerId);
            if (customer == null)
            {
                Console.WriteLine("Error: Selected customer does not exist.");
                return false;
            }

            // Skapa projektentitet från formuläret
            var projectEntity = ProjectFactory.Create(form, customer);
            if (projectEntity == null)
            {
                Console.WriteLine("Error: Failed to create project entity.");
                return false;
            }

            await _projectRepository.AddAsync(projectEntity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateProjectAsync: {ex.Message}");
    
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return false;
        }
    }




    // ==================================================
    //                   READ PROJECT
    // ==================================================

    /// <summary>
    /// Retrieves all projects from the database.
    /// </summary>
    public async Task<IEnumerable<Project?>> GetProjectsAsync()
    {
        try
        {
            var projectEntities = await _projectRepository.GetAllWithCustomerAsync();
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



    /// <summary>
    /// Retrieves a specific project by its ID.
    /// </summary>
    public async Task<Project?> GetProjectByIdAsync(int id)
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



    /// <summary>
    /// Retrieves all projects associated with a specific customer ID.
    /// </summary>
    public async Task<IEnumerable<Project>> GetProjectsByCustomerIdAsync(int customerId)
    {
        var projectEntities = await _projectRepository.GetAllWithCustomerAsync();
        
        return projectEntities
            .Where(p => p.CustomerId == customerId)
            .Select(ProjectFactory.Create)
            .OfType<Project>()
            .ToList();
    }



    /// <summary>
    /// Retrieves projects based on a search term matching the customer's name or email.
    /// </summary>
    public async Task<IEnumerable<Project>> GetProjectsByCustomerNameOrEmailAsync(string searchTerm)
    {
        var projectEntities = await _projectRepository.GetAllWithCustomerAsync();

        return projectEntities
            .Where(x => x.Customer != null &&
                (x.Customer.Name.Contains(searchTerm) ||
                x.Customer.Email!.Contains(searchTerm)))
            .Select(ProjectFactory.Create)
            .OfType<Project>()
            .ToList();        
    }




    // ==================================================
    //                  UPDATE PROJECT
    // ==================================================

    /// <summary>
    /// Updates an existing project's details.
    /// </summary>
    public async Task<bool> UpdateProjectAsync(int id, string title, string? description, DateTime? startDate, DateTime? endDate, ProjectStatus status, List<int>? employeeIds)
    {
        try
        {
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            if (projectEntity == null)
                return false;

            bool hasChanges = false;

            // Uppdatera projektinformation
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
            if (startDate.HasValue)
            {
                projectEntity.StartDate = startDate.Value;
                hasChanges = true;
            }                        
            if (endDate.HasValue)
            {
                projectEntity.EndDate = endDate.Value;
                hasChanges = true;
            }

            projectEntity.Status = status;
            hasChanges = true;

            // Om inga förändringar gjordes, avbryt
            if (!hasChanges && employeeIds == null)
                return false;

            
            // Uppdatera anställda om en ny lista skickas in
            if (employeeIds != null)
            {
                // Tar bort alla befintliga kopplingar mellan projekt och anställda
                await _projectEmployeeRepository.RemoveAllEmployeesFromProjectAsync(projectEntity.Id);

                // Tilldelar de nya anställda
                foreach (var employeeId in employeeIds)
                {
                    var projectEmployee = new ProjectEmployee
                    {
                        ProjectId = projectEntity.Id,
                        EmployeeId = employeeId
                    };

                    await _projectEmployeeRepository.AddAsync(projectEmployee);
                }
            }

            var updatedProjectEntity = await _projectRepository.UpdateAsync(projectEntity);
            return updatedProjectEntity != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateProjectAsync: {ex.Message}");
            return false;
        }
    }




    // ==================================================
    //                  DELETE PROJECT
    // ==================================================

    /// <summary>
    /// Removes a project from the database.
    /// </summary>
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




    // ==================================================
    //          CUSTOMER SELECTION FOR PROJECTS
    // ==================================================

    /// <summary>
    /// Retrieves a list of available customers to assign to projects.
    /// </summary>
    public async Task<List<Customer>> GetAvailableCustomersAsync()
    {
        var customerEntities = await _customerRepository.GetAllAsync();
        return customerEntities.Select(c => new Customer
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email
        }).ToList();
    }




    // ==================================================
    //       EMPLOYEE ASSIGNMENT FOR PROJECTS
    // ==================================================

    /// <summary>
    /// Assigns employees to a project.
    /// </summary>
    public async Task<bool> AssignEmployeesToProjectAsync(int projectId, List<int> employeeIds)
    {
        var project = await _projectRepository.GetOneAsync(p => p.Id == projectId);
        if (project == null) return false;

        var employees = (await _employeeRepository.GetAllAsync(e => employeeIds.Contains(e.Id))).ToList();
        if (!employees.Any()) return false;

        foreach (var employee in employees)
        {
            var projectEmployee = new ProjectEmployee
            {
                ProjectId = projectId,
                EmployeeId = employee.Id
            };

            await _projectEmployeeRepository.AddAsync(projectEmployee);
        }

        return true;
    }



    /// <summary>
    /// Removes an employee from a project.
    /// </summary>
    public async Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId)
    {
        return await _projectEmployeeRepository.RemoveEmployeeFromProjectAsync(projectId, employeeId);
    }
}