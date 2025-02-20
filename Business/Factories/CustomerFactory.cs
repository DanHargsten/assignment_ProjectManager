using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class CustomerFactory
{
    public static CustomerEntity? Create(CustomerRegistrationForm form)
    {
        return form == null ? null : new()
        {
            Name = form.Name,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber
        };
    }


    public static Customer? Create(CustomerEntity entity)
    {
        return entity == null ? null : new Customer
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            CustomerId = entity.Id
        };
    }
}