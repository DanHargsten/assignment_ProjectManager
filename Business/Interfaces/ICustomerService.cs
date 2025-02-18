using Business.Models;

namespace Business.Interfaces
{
    public interface ICustomerService
    {
        Task<bool> CreateCustomerAsync(CustomerRegistrationForm form);
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<IEnumerable<Customer?>> GetCustomersAsync();
        Task<bool> RemoveCustomerAsync(int id);
        Task<bool> UpdateCustomerAsync(int id, string name, string email, string phone);
    }
}