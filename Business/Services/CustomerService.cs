using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;

namespace Business.Services;

/// <summary>
/// Provides operations for managing customers, including creation, retrieval, updates, and deletion.
/// </summary>
public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    private readonly ICustomerRepository _customerRepository = customerRepository;


    // ==================================================
    //                  CREATE CUSTOMER
    // ==================================================

    /// <summary>
    /// Creates a new customer in the database.
    /// </summary>
    /// <param name="form">The customer registration form containing customer details.</param>
    /// <returns>
    /// Returns <c>true</c> if the customer was successfully created, otherwise <c>false</c>.
    /// </returns>
    public async Task<bool> CreateCustomerAsync(CustomerRegistrationForm form)
    {
        try
        {
            if (form == null) return false;

            var customerEntity = CustomerFactory.Create(form);
            if (customerEntity == null) return false;

            await _customerRepository.AddAsync(customerEntity!);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in CreateCustomerAsync: {ex.Message}");
            return false;
        }
    }



    // ==================================================
    //                   READ CUSTOMER
    // ==================================================

    /// <summary>
    /// Retrieves all customers from the database.
    /// </summary>
    /// <returns>
    /// A list of customers. If no customers exist, returns an empty list.
    /// </returns>
    public async Task<IEnumerable<Customer?>> GetCustomersAsync()
    {
        try
        {
            var customerEntities = await _customerRepository.GetAllAsync();
            if (!customerEntities.Any())
                return [];

            return customerEntities
                .Select(CustomerFactory.Create)
                .Where(customer => customer != null)!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failure in GetCustomersAsync: {ex.Message}");
            return [];
        }
    }


    /// <summary>
    /// Retrieves a customer by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <returns>
    /// The customer object if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        try
        {
            var customerEntity = await _customerRepository.GetOneAsync(x => x.Id == id);
            return CustomerFactory.Create(customerEntity!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCustomersByIdAsync: {ex.Message}");
            return null;
        }
    }



    // ==================================================
    //                  UPDATE CUSTOMER
    // ==================================================

    /// <summary>
    /// Updates an existing customer's details.
    /// </summary>
    /// <param name="id">The unique identifier of the customer to update.</param>
    /// <param name="name">The new name of the customer.</param>
    /// <param name="email">The new email of the customer.</param>
    /// <param name="phone">The new phone number of the customer.</param>
    /// <returns>
    /// Returns <c>true</c> if the update was successful, otherwise <c>false</c>.
    /// </returns>
    public async Task<bool> UpdateCustomerAsync(int id, string name, string email, string phone)
    {
        try
        {
            var customerEntity = await _customerRepository.GetOneAsync(x => x.Id == id);
            if (customerEntity == null)
                return false;

            bool hasChanges = false;

            // Uppdaterar namn om det angivits
            if (!string.IsNullOrWhiteSpace(name))
            {
                customerEntity.Name = name;
                hasChanges = true;
            }

            // Uppdaterar e-post om den angivits och inte redan existerar i systemet
            if (!string.IsNullOrWhiteSpace(email) && email != customerEntity.Email)
            {
                var existingCustomer = await _customerRepository.GetOneAsync(c => c.Email == email && c.Id != id);
                if (existingCustomer != null)
                    return false;

                customerEntity.Email = email;
                hasChanges = true;
            }

            // Uppdaterar telefonnummer om det angivits
            if (!string.IsNullOrWhiteSpace(phone))
            {
                customerEntity.PhoneNumber = phone;
                hasChanges = true;
            }

            if (!hasChanges) return false;

            var updatedCustomerEntity = await _customerRepository.UpdateAsync(customerEntity);
            return updatedCustomerEntity != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateCustomerAsync: {ex.Message}");
            return false;
        }
    }



    // ==================================================
    //                  DELETE CUSTOMER
    // ==================================================

    /// <summary>
    /// Removes a customer from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the customer to be removed.</param>
    /// <returns>
    /// Returns <c>true</c> if the customer was successfully removed; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> RemoveCustomerAsync(int id)
    {
        try
        {
            // Hämtar kunden baserat på ID från databasen
            var customerEntity = await _customerRepository.GetOneAsync(x => x.Id == id);
            if (customerEntity == null) return false;

            await _customerRepository.DeleteAsync(customerEntity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RemoveCustomerAsync: {ex.Message}");
            return false;
        }
    }
}