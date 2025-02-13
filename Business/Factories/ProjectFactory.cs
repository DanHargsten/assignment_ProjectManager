using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class ProjectFactory
{
    public static ProjectEntity? Create(ProjectRegistrationForm form, CustomerEntity customer) => form == null ? null : new()
    {
        Title = form.Title,
        Description = form.Description,
        StartDate = form.StartDate,
        EndDate = form.EndDate,
        Status = form.Status,
        CustomerId = customer.Id,
        Customer = customer
    };

    public static Project? Create(ProjectEntity entity) => entity == null ? null : new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        Status = entity.Status,
        CustomerId = entity.CustomerId
    };
}