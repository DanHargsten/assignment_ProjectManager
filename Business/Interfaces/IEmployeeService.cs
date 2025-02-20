using Business.Models;
using Data.Enums;

namespace Business.Interfaces;

public interface IEmployeeService
{
    Task<bool> CreateEmployeeAsync(EmployeeRegistrationForm form);

    Task<IEnumerable<Employee?>> GetEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);

    Task<bool> UpdateEmployeeAsync(int id, string firstName, string lastName, string email, string phone, EmployeeRole role);

    Task<bool> RemoveEmployeeAsync(int id);
}