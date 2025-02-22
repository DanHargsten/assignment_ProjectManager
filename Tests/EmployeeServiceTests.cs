using Business.Models;
using Business.Services;
using Data.Contexts;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Data.Enums;

namespace Tests;

public class EmployeeServiceTests
{
    private static async Task<DataContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
            .Options;

        var dbContext = new DataContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        return dbContext;
    }

    private static async Task<EmployeeService> GetEmployeeServiceAsync()
    {
        var dbContext = await GetDatabaseContext();
        var repository = new EmployeeRepository(dbContext);
        return new EmployeeService(repository);
    }




    // ===========================================
    //              CREATE EMPLOYEE
    // ===========================================

    /// <summary>
    /// Ensures that creating a new employee adds it to the database.
    /// </summary>
    [Fact]
    public async Task CreateEmployeeAsync_ShouldAddEmployee()
    {
        // Arrange
        var service = await GetEmployeeServiceAsync();
        var employeeForm = new EmployeeRegistrationForm
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@domain.com",
            Phone = "123456789",
            Role = EmployeeRole.Developer
        };

        // Act
        bool result = await service.CreateEmployeeAsync(employeeForm);
        var employees = await service.GetEmployeesAsync();

        // Assert
        Assert.True(result);
        Assert.Single(employees);
        Assert.Contains(employees, e =>
            e != null &&
            e.FirstName == "John" &&
            e.LastName == "Doe" &&
            (e.Email ?? "") == "john.doe@domain.com" &&
            (e.Phone ?? "") == "123456789" &&
            e.Role == EmployeeRole.Developer);
    }




    // ===========================================
    //              VIEW EMPLOYEES
    // ===========================================

    /// <summary>
    /// Ensures that retrieving employees returns an empty list when no employees exist.
    /// </summary>
    [Fact]
    public async Task GetEmployeesAsync_ShouldReturnEmptyList_WhenNoEmployeesExist()
    {
        // Arrange
        var service = await GetEmployeeServiceAsync();

        // Act
        var employees = await service.GetEmployeesAsync();

        // Assert
        Assert.NotNull(employees);
        Assert.Empty(employees);
    }




    // ===========================================
    //              UPDATE EMPLOYEE
    // ===========================================

    /// <summary>
    /// Ensures that updating an existing employee correctly modifies their details.
    /// </summary>
    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdateEmployee()
    {
        // Arrange
        var service = await GetEmployeeServiceAsync();
        var employeeForm = new EmployeeRegistrationForm
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@domain.com",
            Phone = "123456789",
            Role = EmployeeRole.Manager
        };

        await service.CreateEmployeeAsync(employeeForm);
        var employee = (await service.GetEmployeesAsync()).FirstOrDefault();
        Assert.NotNull(employee);

        // Act
        bool result = await service.UpdateEmployeeAsync(employee!.Id, "Jane", "Doe", "jane.doe@domain.com", "987654321", EmployeeRole.Designer);
        var updatedEmployee = (await service.GetEmployeesAsync()).FirstOrDefault(e => e!.Id == employee.Id);
        Assert.NotNull(updatedEmployee);

        // Assert
        Assert.True(result);
        Assert.Equal("Jane", updatedEmployee!.FirstName);
        Assert.Equal("jane.doe@domain.com", updatedEmployee.Email);
        Assert.Equal("987654321", updatedEmployee.Phone);
        Assert.Equal(EmployeeRole.Designer, updatedEmployee.Role);
    }


    /// <summary>
    /// Ensures that updating an employee to an invalid role fails.
    /// </summary>
    [Fact]
    public async Task UpdateEmployeeAsync_ShouldFail_WhenInvalidRoleIsGiven()
    {
        // Arrange
        var service = await GetEmployeeServiceAsync();
        var employeeForm = new EmployeeRegistrationForm { FirstName = "Test", LastName = "User", Role = EmployeeRole.Developer };

        await service.CreateEmployeeAsync(employeeForm);
        var employee = (await service.GetEmployeesAsync()).FirstOrDefault()!;

        // Act
        bool result = await service.UpdateEmployeeAsync(employee.Id, "Updated", "User", "updated@domain.com", "987654321", (EmployeeRole)999);

        // Assert
        Assert.False(result);
    }


    /// <summary>
    /// Ensures that updating an employee's role is successful.
    /// </summary>
    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdateRoleSuccessfully()
    {
        // Arrange
        var service = await GetEmployeeServiceAsync();
        var employeeForm = new EmployeeRegistrationForm { FirstName = "Dan", LastName = "Hargsten", Role = EmployeeRole.Developer };

        await service.CreateEmployeeAsync(employeeForm);
        var employee = (await service.GetEmployeesAsync()).FirstOrDefault()!;

        // Act
        bool result = await service.UpdateEmployeeAsync(employee.Id, "Dan", "Hargsten", "dh@domain.com", "555-1234", EmployeeRole.Manager);
        var updatedEmployee = (await service.GetEmployeesAsync()).FirstOrDefault(e => e!.Id == employee.Id);

        // Assert
        Assert.True(result);
        Assert.NotNull(updatedEmployee);
        Assert.Equal(EmployeeRole.Manager, updatedEmployee!.Role);
    }


    /// <summary>
    /// Ensures that attempting to update a non-existing employee does not cause an error.
    /// </summary>
    [Fact]
    public async Task UpdateEmployeeAsync_ShouldNotUpdateNonExistingEmployee()
    {
        // Arrange
        var service = await GetEmployeeServiceAsync();

        // Act
        bool result = await service.UpdateEmployeeAsync(9999, "Fake", "Fejkarsson", "fake@domain.com", "000000000", EmployeeRole.Manager);

        // Assert
        Assert.False(result);
    }




    // ===========================================
    //              DELETE EMPLOYEE
    // ===========================================

    /// <summary>
    /// Ensures that an employee can be successfully removed.
    /// </summary>
    [Fact]
    public async Task RemoveEmployeeAsync_ShouldDeleteEmployee()
    {
        // Arrange
        var service = await GetEmployeeServiceAsync();
        var employeeForm = new EmployeeRegistrationForm
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@domain.com",
            Phone = "987654321",
            Role = EmployeeRole.Developer
        };

        await service.CreateEmployeeAsync(employeeForm);
        var employee = (await service.GetEmployeesAsync()).FirstOrDefault() ?? throw new InvalidOperationException("..");

        // Act
        bool result = await service.RemoveEmployeeAsync(employee.Id);
        var employees = await service.GetEmployeesAsync();

        // Assert
        Assert.True(result);
        Assert.Empty(employees);
    }


    /// <summary>
    /// Ensures that attempting to remove a non-existing employee does not cause an error.
    /// </summary>
    [Fact]
    public async Task RemoveEmployeeAsync_ShouldNotRemoveNonExistingEmployee()
    {
        // Arrange
        var service = await GetEmployeeServiceAsync();

        // Act
        bool result = await service.RemoveEmployeeAsync(9999);

        // Assert
        Assert.False(result);
    }
}