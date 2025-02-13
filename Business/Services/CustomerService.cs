using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;

namespace Business.Services;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    private readonly ICustomerRepository _customerRepository = customerRepository;


    // CREATE //
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
            Console.WriteLine($"Error in CreateCustomerAsync: {ex.Message}");
            return false;
        }
    }


    // READ //
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
            Console.WriteLine($"Error in GetCustomersAsync: {ex.Message}");
            return [];
        }
    }

    public async Task<Customer?> GetCustomerById(int id)
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


    // UPDATE //
    public async Task<bool> UpdateCustomerAsync(int id, string name, string email, string phone)
    {
        try
        {
            var customerEntity = await _customerRepository.GetOneAsync(x => x.Id == id);
            if (customerEntity == null)
                return false;

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(name))
            {
                customerEntity.Name = name;
                hasChanges = true;
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                customerEntity.Email = email;
                hasChanges = true;
            }
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


    // DELETE //
    public async Task<bool> RemoveCustomerAsync(int id)
    {
        try
        {
            var customerEntity = await _customerRepository.GetOneAsync(x => x.Id == id);
            if (customerEntity == null) return true;

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