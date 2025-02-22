using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Data.Interfaces;

namespace Business.Services;


/// <summary>
/// Provides operations for managing employees, including creation, retrieval, updates, and deletion.
/// </summary>
public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;


    // ==================================================
    //                  CREATE EMPLOYEE
    // ==================================================

    /// <summary>
    /// Creates a new employee in the database.
    /// </summary>
    /// <param name="form">The registration form containing employee details.</param>
    /// <returns>
    /// Returns <c>true</c> if the employee was successfully created; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> CreateEmployeeAsync(EmployeeRegistrationForm form)
    {
        try
        {
            if (form == null) return false;

            var employeeEntity = EmployeeFactory.Create(form);
            if (employeeEntity == null) return false;

            await _employeeRepository.AddAsync(employeeEntity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in CreateEmployeeAsync: {ex.Message}");
            return false;
        }
    }



    // ==================================================
    //                   READ EMPLOYEE
    // ==================================================

    /// <summary>
    /// Retrieves all employees from the database.
    /// </summary>
    /// <returns>
    /// Returns a collection of employees, or an empty list if no employees exist.
    /// </returns>
    public async Task<IEnumerable<Employee?>> GetEmployeesAsync()
    {
        try
        {
            var employeeEntities = await _employeeRepository.GetAllAsync();
            if (!employeeEntities.Any())
                return [];

            return employeeEntities
                .Select(EmployeeFactory.Create)
                .Where(employee => employee != null)!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in GetEmployeesAsync: {ex.Message}");
            return [];
        }
    }


    /// <summary>
    /// Retrieves an employee by ID.
    /// </summary>
    /// <param name="id">The ID of the employee to retrieve.</param>
    /// <returns>
    /// Returns an <see cref="Employee"/> object if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        try
        {
            // Hämtar en anställd baserat på ID
            var employeeEntity = await _employeeRepository.GetOneAsync(x => x.Id == id);

            return EmployeeFactory.Create(employeeEntity!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in GetOneAsync: {ex.Message}");
            return null;
        }
    }



    // ==================================================
    //                  UPDATE EMPLOYEE
    // ==================================================

    /// <summary>
    /// Updates an existing employee's details.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="firstName">The updated first name.</param>
    /// <param name="lastName">The updated last name.</param>
    /// <param name="email">The updated email address.</param>
    /// <param name="phone">The updated phone number.</param>
    /// <param name="role">The updated employee role.</param>
    /// <returns>
    /// Returns <c>true</c> if the update was successful; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> UpdateEmployeeAsync(int id, string firstName, string lastName, string email, string phone, EmployeeRole role)
    {
        try
        {
            // Hämtar den anställde baserat på ID
            var employeeEntity = await _employeeRepository.GetOneAsync(x => x.Id == id);
            if (employeeEntity == null)
                return false;

            // Validerar att rollen är giltig
            if (!Enum.IsDefined(role))
                return false;

            bool hasChanges = false;

            // Uppdaterar fälten om de innehåller värden
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                employeeEntity.FirstName = firstName;
                hasChanges = true;
            }
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                employeeEntity.LastName = lastName;
                hasChanges = true;
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                employeeEntity.Email = email;
                hasChanges = true;
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                employeeEntity.Phone = phone;
                hasChanges = true;
            }

            // Uppdaterar rollen
            employeeEntity.Role = role;
            hasChanges = true;

            if (!hasChanges) return false;

            // Uppdaterar den anställde i databasen
            var updatedEmployeeEntity = await _employeeRepository.UpdateAsync(employeeEntity);
            return updatedEmployeeEntity != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in UpdateEmployeeAsync: {ex.Message}");
            return false;
        }
    }



    // ==================================================
    //                  DELETE EMPLOYEE
    // ==================================================

    /// <summary>
    /// Removes an employee from the database.
    /// </summary>
    /// <param name="id">The ID of the employee to remove.</param>
    /// <returns>
    /// Returns <c>true</c> if the employee was successfully removed; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> RemoveEmployeeAsync(int id)
    {
        try
        {
            // Hämtar den anställde baserat på ID
            var employeeEntity = await _employeeRepository.GetOneAsync(x => x.Id == id);
            if (employeeEntity == null)
                return false;

            await _employeeRepository.DeleteAsync(employeeEntity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in RemoveEmployeeAsync: {ex.Message}");
            return false;
        }
    }
}