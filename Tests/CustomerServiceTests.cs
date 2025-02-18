using Business.Models;
using Business.Services;
using Data.Contexts;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class CustomerServiceTests
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

    private static async Task<CustomerService> GetCustomerServiceAsync()
    {
        var dbContext = await GetDatabaseContext();
        var repository = new CustomerRepository(dbContext);
        return new CustomerService(repository);
    }





    /// <summary>
    /// Ensures that creating a new customer adds it to the database.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateCustomerAsync_ShouldAddCustomer()
    {
        // Arrange
        var service = await GetCustomerServiceAsync();
        var customerForm = new CustomerRegistrationForm
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };

        // Act
        bool result = await service.CreateCustomerAsync(customerForm);
        var customers = await service.GetCustomersAsync();

        // Assert
        Assert.True(result);
        Assert.Single(customers);
        Assert.Contains(customers, c =>
            c != null &&
            c.Name == "John Doe" &&
            (c.Email ?? "") == "john.doe@example.com" &&
            (c.PhoneNumber ?? "") == "123456789");
    }



    /// <summary>
    /// Ensures that retrieving customers returns an empty list when no customers exist.
    /// </summary>
    [Fact]
    public async Task GetCustomersAsync_ShouldReturnEmptyList_WhenNoCustomersExist()
    {
        // Arrange
        var service = await GetCustomerServiceAsync();

        // Act
        var customers = await service.GetCustomersAsync();

        // Assert
        Assert.NotNull(customers);
        Assert.Empty(customers);
    }



    /// <summary>
    /// Ensures that updating an existing customer correctly modifies their details.
    /// </summary>
    [Fact]
    public async Task UpdateCustomerAsync_ShouldUpdateCustomer()
    {
        // Arrange
        var service = await GetCustomerServiceAsync();
        var customerForm = new CustomerRegistrationForm
        {
            Name = "Ace",
            Email = "ace@example.com",
            PhoneNumber = "123456789"
        };
        
        await service.CreateCustomerAsync(customerForm);
        var customer = (await service.GetCustomersAsync()).FirstOrDefault();

        Assert.NotNull(customer);

        // Act
        bool result = await service.UpdateCustomerAsync(customer!.Id, "Acme", "acme@example.com", "987654321");
        var updatedCustomer = (await service.GetCustomersAsync()).FirstOrDefault(c => c.Id == customer.Id);

        // Assert
        Assert.True(result);
        Assert.NotNull(updatedCustomer);
        Assert.Equal("Acme", updatedCustomer!.Name);
        Assert.Equal("acme@example.com", updatedCustomer.Email);
        Assert.Equal("987654321", updatedCustomer.PhoneNumber);
    }



    /// <summary>
    /// Ensures that attempting to update a non-existing customer does not cause an error.
    /// </summary>
    [Fact]
    public async Task UpdateCustomerAsync_ShouldNotUpdateNonExistingCustomer()
    {
        // Arrange
        var service = await GetCustomerServiceAsync();

        // Act
        bool result = await service.UpdateCustomerAsync(9999, "Fake Name", "fake@example.com", "000000000");

        // Assert
        Assert.False(result);
    }



    /// <summary>
    /// Ensures that a customer can be successfully removed.
    /// </summary>
    [Fact]
    public async Task RemoveCustomerAsync_ShouldDeleteCustomer()
    {
        // Arrange
        var service = await GetCustomerServiceAsync();
        var customerForm = new CustomerRegistrationForm { Name = "Jane Doe", Email = "jane.doe@example.com", PhoneNumber = "987654321" };
        await service.CreateCustomerAsync(customerForm);
        var customer = (await service.GetCustomersAsync()).FirstOrDefault() ?? throw new InvalidOperationException("..");

        // Act
        bool result = await service.RemoveCustomerAsync(customer.Id);
        var customers = await service.GetCustomersAsync();

        // Assert
        Assert.True(result);
        Assert.Empty(customers);
    }



    /// <summary>
    /// Ensures that attempting to remove a non-existing customer does not cause an error.
    /// </summary>
    [Fact]
    public async Task RemoveCustomerAsync_ShouldNotRemoveNonExistingCustomer()
    {
        // Arrange
        var service = await GetCustomerServiceAsync();

        // Act
        bool result = await service.RemoveCustomerAsync(9999);

        // Assert
        Assert.False(result);
    }
}