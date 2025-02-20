using Business.Models;
using Data.Entities;
using Data.Enums;

namespace Business.Factories;

public static class EmployeeFactory
{
    public static EmployeeEntity? Create(EmployeeRegistrationForm form)
    {
        return form == null ? null : new()
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            Phone = form.Phone,
            Role = (EmployeeRole)form.Role
        };
    }


    public static Employee? Create(EmployeeEntity entity)
    {
        return entity == null ? null : new Employee
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            Role = entity.Role
        };
    }  
}