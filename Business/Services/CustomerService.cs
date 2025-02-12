using Business.Factories;
using Business.Models;
using Data.Repositories;

namespace Business.Services;

public class CustomerService(CustomerRepository customerRepository)
{
    private readonly CustomerRepository _customerRepository = customerRepository;

    
    public async Task<bool> CreateCustomerAsync(CustomerRegistrationForm form)
    {
        try
        {
            var customerEntity = CustomerFactory.Create(form);
            await _customerRepository.AddAsync(customerEntity!);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateCustomerAsync: {ex.Message}");
            return false;
        }
    }


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
}