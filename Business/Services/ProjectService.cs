﻿using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepository, ICustomerRepository customerRepository) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;


    /// <summary>
    /// Creates a new project and stores it in the database.
    /// </summary>
    /// <param name="form">The project registration form containing project details.</param>
    /// <returns>Returns true if the project was created successfully, otherwise false.</returns>
    public async Task<bool> CreateProjectAsync(ProjectRegistrationForm form)
    {
        try
        {
            // Fetch the customer from the database to ensure it exists
            var customer = await _customerRepository.GetOneAsync(x => x.Id == form.CustomerId);
            if (customer == null)
            {
                Console.WriteLine("Error: Selected customer does not exist.");
                return false;
            }

            // Convert the form data into a ProjectEntity
            var projectEntity = ProjectFactory.Create(form, customer);

            // Ensure the projectEntity is not null before adding it to the database
            if (projectEntity == null)
            {
                Console.WriteLine("Error: Failed to create project entity.");
                return false;
            }

            // Save the new project in the database
            await _projectRepository.AddAsync(projectEntity);

            return true;
        }
        catch (Exception ex)
        {
            // Log any errors that occur during the creation process
            Console.WriteLine($"Error in CreateProjectAsync: {ex.Message}");
    
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return false;
        }
    }


    // READ //
    public async Task<IEnumerable<Project?>> GetProjectsAsync()
    {
        try
        {
            var projectEntities = await _projectRepository.GetAllAsync();
            if (!projectEntities.Any())
                return [];

            return projectEntities
                .Select(ProjectFactory.Create)
                .Where(project => project != null)!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetProjectAsync: {ex.Message}");
            return [];
        }
    }

    public async Task<Project?> GetProjectById(int id)
    {
        try
        {
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            return ProjectFactory.Create(projectEntity!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetProjectByIdAsync: {ex.Message}");
            return null;
        }
    }


    // UPDATE //
    public async Task<bool> UpdateProjectAsync(int id, string title, string? description, string startDate, string? endDate)
    {
        try
        {
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            if (projectEntity == null)
                return false;

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(title))
            {
                projectEntity.Title = title;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                projectEntity.Description = description;
                hasChanges = true;
            }

            // Validate and convert by ChatGPT
            if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParseExact(startDate, "yyyy-mm-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDate))
            {
                projectEntity.StartDate = parsedStartDate;
                hasChanges = true;
            }

            // Validate and convert by ChatGPT
            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParseExact(endDate, "yyyy-mm-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDate))
            {
                projectEntity.EndDate = parsedEndDate;
                hasChanges = true;
            }

            if (!hasChanges) return false;



            var updatedProjectEntity = await _projectRepository.UpdateAsync(projectEntity);
            return updatedProjectEntity != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateCustomerAsync: {ex.Message}");
            return false;
        }
    }


    // DELETE //
    public async Task<bool> RemoveProjectAsync(int id)
    {
        try
        {
            var projectEntity = await _projectRepository.GetOneAsync(x => x.Id == id);
            if (projectEntity == null) return true;

            await _projectRepository.DeleteAsync(projectEntity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RemoveProjectAsync: {ex.Message}");
            return false;
        }
    }



    public async Task<List<Customer>> GetAvailableCustomersAsync()
    {
        var customerEntities = await _customerRepository.GetAllAsync();
        return customerEntities.Select(c => new Customer
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email
        }).ToList();
    }
}