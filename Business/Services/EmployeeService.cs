using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Data.Interfaces;

namespace Business.Services;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    
    // CREATE //
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



    // READ //
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


    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        try
        {
            var employeeEntity = await _employeeRepository.GetOneAsync(x => x.Id == id);
            return EmployeeFactory.Create(employeeEntity!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in GetOneAsync: {ex.Message}");
            return null;
        }
    }


    // UPDATE //
    public async Task<bool> UpdateEmployeeAsync(int id, string firstName, string lastName, string email, string phone, EmployeeRole role)
    {
        try
        {
            var employeeEntity = await _employeeRepository.GetOneAsync(x => x.Id == id);
            if (employeeEntity == null)
                return false;

            if (!Enum.IsDefined(role))
                return false;

            bool hasChanges = false;

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

            employeeEntity.Role = role;
            hasChanges = true;

            if (!hasChanges) return false;

            var updatedEmployeeEntity = await _employeeRepository.UpdateAsync(employeeEntity);
            return updatedEmployeeEntity != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in UpdateEmployeeAsync: {ex.Message}");
            return false;
        }
    }

    // DELETE //
    public async Task<bool> RemoveEmployeeAsync(int id)
    {
        try
        {
            var employeeEntity = await _employeeRepository.GetOneAsync(x => x.Id == id);
            if (employeeEntity == null) return false;

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