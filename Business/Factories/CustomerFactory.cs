using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class CustomerFactory
{
    public static CustomerEntity? Create(CustomerRegistrationForm form) => form == null ? null : new()
    {
        Name = form.Name,
        Email = form.Email,
        PhoneNumber = form.PhoneNumber
    };

    public static Customer? Create(CustomerEntity entity) => entity == null ? null : new()
    { 
        Id = entity.Id,
        Name = entity.Name,
        Email = entity.Email,
        PhoneNumber = entity.PhoneNumber,
        CustomerId = entity.CustomerId
    };
}