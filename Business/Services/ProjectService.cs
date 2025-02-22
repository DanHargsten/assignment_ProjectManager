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
                Console.WriteLine("[ERROR] Selected customer does not exist.");
                return false;
            }

            // Skapa projektentitet från formuläret
            var projectEntity = ProjectFactory.Create(form, customer);
            if (projectEntity == null)
            {
                Console.WriteLine("[ERROR] Failed to create project entity.");
                return false;
            }

            await _projectRepository.AddAsync(projectEntity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in CreateProjectAsync: {ex.Message}");
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
            // Hämtar alla projekt inklusive kundinformation från databasen
            var projectEntities = await _projectRepository.GetAllWithCustomerAsync();
            if (!projectEntities.Any())
                return [];

            return projectEntities
                .Select(ProjectFactory.Create)
                .Where(project => project != null)!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in GetProjectsAsync: {ex.Message}");
            return [];
        }
    }



    /// <summary>
    /// Retrieves a project by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the project to retrieve.</param>
    /// <returns>
    /// Returns the project if found, otherwise null.
    /// </returns>
    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        try
        {
            // Hämtar projektet från databasen baserat på det angivna ID:t
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            return ProjectFactory.Create(projectEntity!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in GetProjectByIdAsync: {ex.Message}");
            return null;
        }
    }



    /// <summary>
    /// Retrieves all projects associated with a specific customer ID.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <returns>
    /// A list of projects that belong to the specified customer.
    /// If no projects are found, returns an empty list.
    /// </returns>
    public async Task<IEnumerable<Project>> GetProjectsByCustomerIdAsync(int customerId)
    {
        try
        {
            // Hämtar alla projekt inklusive kundinformation
            var projectEntities = await _projectRepository.GetAllWithCustomerAsync();

            // Filtrerar ut projekten som tillhör den specifika kunden
            return projectEntities
                .Where(p => p.CustomerId == customerId)
                .Select(ProjectFactory.Create)
                .OfType<Project>()
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in GetProjectsByCustomerIdAsync: {ex.Message}");
            return [];
        }
    }



    /// <summary>
    /// Retrieves all projects associated with a specific customer ID.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer whose projects should be retrieved.</param>
    /// <returns>
    /// Returns a list of projects belonging to the specified customer. 
    /// If no projects are found, an empty list is returned.
    /// </returns>
    public async Task<IEnumerable<Project>> GetProjectsByCustomerNameOrEmailAsync(string searchTerm)
    {
        try
        {
            // Hämtar alla projekt inklusive deras kopplade kunder från databasen
            var projectEntities = await _projectRepository.GetAllWithCustomerAsync();

            // Filtrerar ut projekt som tillhör den angivna kunden
            return projectEntities
                .Where(x => x.Customer != null &&
                    (x.Customer.Name.Contains(searchTerm) ||
                    x.Customer.Email!.Contains(searchTerm)))
                .Select(ProjectFactory.Create)
                .OfType<Project>()
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in GetProjectsByCustomerNameOrEmailAsync: {ex.Message}");
            return [];
        }
    }




    // ==================================================
    //                  UPDATE PROJECT
    // ==================================================

    /// <summary>
    /// Updates an existing project's details, including its title, description, dates, status, and assigned employees.
    /// </summary>
    /// <param name="id">The unique identifier of the project to update.</param>
    /// <param name="title">The updated title of the project.</param>
    /// <param name="description">The updated description of the project (optional).</param>
    /// <param name="startDate">The updated start date of the project (optional).</param>
    /// <param name="endDate">The updated end date of the project (optional).</param>
    /// <param name="status">The updated status of the project.</param>
    /// <param name="employeeIds">A list of employee IDs to be assigned to the project (optional).</param>
    /// <returns>
    /// Returns true if the project was updated successfully; otherwise, false.
    /// </returns>
    public async Task<bool> UpdateProjectAsync(int id, string title, string? description, DateTime? startDate, DateTime? endDate, ProjectStatus status, List<int>? employeeIds)
    {
        try
        {
            // Hämtar projektet från databasen baserat på ID
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            if (projectEntity == null)
                return false;

            bool hasChanges = false;

            // Uppdaterar projektets grundläggande information om nya värden har skickats in
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

            // Uppdaterar projektstatus
            projectEntity.Status = status;
            hasChanges = true;

            // Om inga förändringar gjordes och inga nya anställda skickades in, avbryt
            if (!hasChanges && employeeIds == null)
                return false;


            // Hanterar anställda om en ny lista har skickats in
            if (employeeIds != null)
            {                
                await _projectEmployeeRepository.RemoveAllEmployeesFromProjectAsync(projectEntity.Id);

                // Lägger till de nya anställda i projektet
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
            Console.WriteLine($"[ERROR] Failure in UpdateProjectAsync: {ex.Message}");
            return false;
        }
    }



    // ==================================================
    //                  DELETE PROJECT
    // ==================================================

    /// <summary>
    /// Removes a project from the database based on the provided project ID.
    /// </summary>
    /// <param name="id">The unique identifier of the project to be removed.</param>
    /// <returns>
    /// Returns true if the project was successfully removed or did not exist; otherwise, false if an error occurred.
    /// </returns>
    public async Task<bool> RemoveProjectAsync(int id)
    {
        try
        {
            // Försöker hämta projektet från databasen
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            if (projectEntity == null)
                return true;

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
    /// Retrieves a list of available customers that can be assigned to projects.
    /// </summary>
    /// <returns>
    /// A list of customers containing their ID, name, and email.
    /// </returns>
    public async Task<List<Customer>> GetAvailableCustomersAsync()
    {
        try
        {
            var customerEntities = await _customerRepository.GetAllAsync();
            return customerEntities.Select(c => new Customer
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in GetAvailableCustomersAsync: {ex.Message}");
            return [];
        }
    }




    // ==================================================
    //       EMPLOYEE ASSIGNMENT FOR PROJECTS
    // ==================================================

    /// <summary>
    /// Assigns employees to a project.
    /// </summary>
    /// <param name="projectId">The ID of the project to assign employees to.</param>
    /// <param name="employeeIds">A list of employee IDs to be assigned to the project.</param>
    /// <returns>True if employees were successfully assigned, otherwise false.</returns>
    public async Task<bool> AssignEmployeesToProjectAsync(int projectId, List<int> employeeIds)
    {
        try
        {
            // Hämtar projektet baserat på projektets ID
            var project = await _projectRepository.GetOneAsync(p => p.Id == projectId);
            if (project == null)
                return false;

            // Hämtar alla anställda vars ID finns i listan
            var employees = (await _employeeRepository.GetAllAsync(e => employeeIds.Contains(e.Id))).ToList();
            if (employees.Count == 0)
                return false;

            // Itererar genom alla anställda och skapar en koppling till projektet
            foreach (var employee in employees)
            {
                var projectEmployee = new ProjectEmployee
                {
                    ProjectId = projectId,
                    EmployeeId = employee.Id
                };

                // Lägger till kopplingen i databasen
                await _projectEmployeeRepository.AddAsync(projectEmployee);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in AssignEmployeesToProjectAsync: {ex.Message}");
            return false;
        }
    }



    /// <summary>
    /// Removes an employee from a specific project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="employeeId">The unique identifier of the employee to be removed.</param>
    /// <returns>
    /// Returns true if the employee was successfully removed; otherwise, false.
    /// </returns>
    public async Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId)
    {
        try
        {
            return await _projectEmployeeRepository.RemoveEmployeeFromProjectAsync(projectId, employeeId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in RemoveEmployeeFromProjectAsync: {ex.Message}");
            return false;
        }
    }
}