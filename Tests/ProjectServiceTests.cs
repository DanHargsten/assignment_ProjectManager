using Business.Models;
using Business.Services;
using Data.Contexts;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class ProjectServiceTests
{
    private readonly DataContext _dbContext = GetDatabaseContext().Result;

    
    private static async Task<DataContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var dbContext = new DataContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        return dbContext;
    }


    private static ProjectService GetProjectServiceAsync(DataContext dbContext)
    {
        var projectRepository = new ProjectRepository(dbContext);
        var customerRepository = new CustomerRepository(dbContext);
        return new ProjectService(projectRepository, customerRepository);
    }


    private static CustomerService GetCustomerService(DataContext dbContext)
    {
        var repository = new CustomerRepository(dbContext);
        return new CustomerService(repository);
    }




    /// <summary>
    /// Ensures that a project can be successfully created.
    /// </summary>
    [Fact]
    public async Task CreateProjectAsync_ShouldAddProject()
    {
        // Arrange
        await ResetDatabaseAsync();

        var service = GetProjectServiceAsync(_dbContext);
        var customerService = GetCustomerService(_dbContext);

        var customerForm = new CustomerRegistrationForm
        {
            Name = "John Doe",
            Email = "john.doe@domain.com",
        };

        await customerService.CreateCustomerAsync(customerForm);
        var customer = (await customerService.GetCustomersAsync()).First();

        var projectForm = new ProjectRegistrationForm
        {
            Title = "New Project",
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Status = Data.Enums.ProjectStatus.NotStarted,
            CustomerId = customer!.Id
        };


        // Act
        bool result = await service.CreateProjectAsync(projectForm);
        var projects = await service.GetProjectsAsync();


        // Assert
        Assert.True(result);
        Assert.Single(projects);
        Assert.Contains(projects, p =>
            p != null &&
            p.Title == "New Project" &&
            p.Description == "Test Description" &&
            p.CustomerId == customer.Id);
    }



    /// <summary>
    /// Ensures that retrieving all projects returns the correct projects.
    /// </summary>
    [Fact]
    public async Task GetProjectsAsync_ShouldReturnAllProjects()
    {
        // Arrange
        await ResetDatabaseAsync();

        var service = GetProjectServiceAsync(_dbContext);
        var customerService = GetCustomerService(_dbContext);


        var customerForm = new CustomerRegistrationForm { Name = "Test Customer", Email = "test@domain.com" };
        await customerService.CreateCustomerAsync(customerForm);

        var customer = (await customerService.GetCustomersAsync()).FirstOrDefault()!;

        var projectForm1 = new ProjectRegistrationForm { Title = "Project 1", CustomerId = customer.Id, StartDate = DateTime.UtcNow };
        var projectForm2 = new ProjectRegistrationForm { Title = "Project 2", CustomerId = customer.Id, StartDate = DateTime.UtcNow };

        await service.CreateProjectAsync(projectForm1);
        await service.CreateProjectAsync(projectForm2);

        // Act
        var projects = await service.GetProjectsAsync();

        // Assert
        Assert.NotNull(projects);
        Assert.Equal(2, projects.Count());
        Assert.Contains(projects, p => p!.Title == "Project 1");
        Assert.Contains(projects, p => p!.Title == "Project 2");
    }



    /// <summary>
    /// Ensures that retrieving projects when none exist returns an empty list.
    /// </summary>
    [Fact]
    public async Task GetProjectsAsync_ShouldReturnEmptyList_WhenNoProjectsExist()
    {
        // Arrange
        await ResetDatabaseAsync();
        var service = GetProjectServiceAsync(_dbContext);

        // Act
        var projects = await service.GetProjectsAsync();

        // Assert
        Assert.NotNull(projects);
        Assert.Empty(projects);
    }



    /// <summary>
    /// Ensures that a project can be successfully updated.
    /// </summary>
    [Fact]
    public async Task UpdateProjectAsync_ShouldUpdateProject()
    {
        // Arrange
        await ResetDatabaseAsync();

        var service = GetProjectServiceAsync(_dbContext);
        var customerService = GetCustomerService(_dbContext);

        var customerForm = new CustomerRegistrationForm { Name = "Test Customer", Email = "test@domain.com" };
        await customerService.CreateCustomerAsync(customerForm);
        var customer = (await customerService.GetCustomersAsync()).FirstOrDefault()!;

        var projectForm = new ProjectRegistrationForm { Title = "Old Title", CustomerId = customer.Id, StartDate = DateTime.UtcNow };
        await service.CreateProjectAsync(projectForm);

        var project = (await service.GetProjectsAsync()).FirstOrDefault()!;
        var updatedTitle = "Updated Title";

        // Act
        bool result = await service.UpdateProjectAsync(project.Id, updatedTitle, null, project.StartDate.ToString("yyyy-MM-dd"), null, "NotStarted");
        var updatedProject = (await service.GetProjectsAsync()).FirstOrDefault(p => p!.Id == project.Id)!;

        // Assert
        Assert.True(result);
        Assert.NotNull(updatedProject);
        Assert.Equal(updatedTitle, updatedProject.Title);
    }



    /// <summary>
    /// Ensures that updating a non-existing project returns false.
    /// </summary>
    [Fact]
    public async Task UpdateProjectAsync_ShouldFail_WhenProjectDoesNotExist()
    {
        // Arrange
        await ResetDatabaseAsync();

        var service = GetProjectServiceAsync(_dbContext);

        // Act
        bool result = await service.UpdateProjectAsync(999, "Title", null, "2024-01-01", null, "NotStarted");

        // Assert
        Assert.False(result);
    }



    /// <summary>
    /// Ensures that a project can be successfully deleted.
    /// </summary>
    [Fact]
    public async Task RemoveProjectAsync_ShouldDeleteProject()
    {
        // Arrange
        await ResetDatabaseAsync();

        var service = GetProjectServiceAsync(_dbContext);
        var customerService = GetCustomerService(_dbContext);

        var customerForm = new CustomerRegistrationForm { Name = "Test Customer", Email = "test@domain.com" };
        await customerService.CreateCustomerAsync(customerForm);
        var customer = (await customerService.GetCustomersAsync()).FirstOrDefault()!;

        var projectForm = new ProjectRegistrationForm { Title = "Project to Delete", CustomerId = customer.Id, StartDate = DateTime.UtcNow };
        await service.CreateProjectAsync(projectForm);

        var project = (await service.GetProjectsAsync()).FirstOrDefault()!;

        // Act
        bool result = await service.RemoveProjectAsync(project.Id);
        var projectsAfterDelete = await service.GetProjectsAsync();

        // Assert
        Assert.True(result);
        Assert.DoesNotContain(projectsAfterDelete, p => p!.Id == project.Id);
    }



    /// <summary>
    /// Ensures that deleting a non-existing project does not cause failure.
    /// </summary>
    [Fact]
    public async Task RemoveProjectAsync_ShouldNotFail_WhenProjectDoesNotExist()
    {
        // Arrange
        await ResetDatabaseAsync();

        var service = GetProjectServiceAsync(_dbContext);

        // Act
        bool result = await service.RemoveProjectAsync(999);

        // Assert
        Assert.True(result);
    }





    /// <summary>
    /// Resets the database before each test to ensure a clean state.
    /// </summary>
    private async Task ResetDatabaseAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync(); 
        await _dbContext.Database.EnsureCreatedAsync(); 
    }

}