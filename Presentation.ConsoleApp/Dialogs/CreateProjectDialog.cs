using Business.Interfaces;
using Business.Models;
using Data.Enums;

namespace Presentation.ConsoleApp.Dialogs;

/// <summary>
/// Handles user input for creating a new project and passes it to the ProjectService.
/// </summary>
public class CreateProjectDialog(IProjectService projectService, ICustomerService customerService)
{
    private readonly IProjectService _projectService = projectService;
    private readonly ICustomerService _customerService = customerService;

    #region Main Execution
    /// <summary>
    /// Handles user input to create a new project.
    /// Uses ProjectService to perform the actual creation.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("------   CREATE NEW PROJECT    ------");
        Console.WriteLine("-------------------------------------\n");
        Console.WriteLine("* = optional input, press Enter to skip\n");


        // Få användarinput för projektdetaljerna
        string title = GetUserInput("Enter project title: ");
        string? description = GetOptionalUserInput("* Enter project description: ");

        // Start- och slutdatum från användaren
        DateTime startDate = GetDateInput("Enter project start date (YYYY-MM-DD): ");
        DateTime? endDate = GetNullableDateInput("* Enter project end date (YYYY-MM-DD): ");
        

        // Listar befintliga kunder
        var customers = await _customerService.GetCustomersAsync();
        if (!customers.Any())
        {
            Console.WriteLine("No customers available. Please add a customer first.");
            return;
        }

        var selectedCustomer = SelectCustomer(customers!);
        if (selectedCustomer == null) return;

        // Listar projektstatusar (Not Started, In Progress, Paused, Completed)
        var selectedStatus = SelectProjectStatus();
        if (selectedStatus == null) return;


        // Skapa projektformulärobjektet
        var form = new ProjectRegistrationForm
        {
            Title = title,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            Status = selectedStatus.Value,
            CustomerId = selectedCustomer.Id
        };


        // Skicka formuläret till ProjectService för att skapa projektet
        var success = await _projectService.CreateProjectAsync(form);

        Console.Clear();
        Console.WriteLine(success ? "Project created successfully!" : "Failed to create project.");
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
    #endregion




    #region Helper Methods
    /// <summary>
    /// Prompts user for input and ensures it's not empty.
    /// </summary>
    private static string GetUserInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!.Trim();

            if (!string.IsNullOrWhiteSpace(input))            
                return input;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Can't leave this field empty. Please enter a value.\n");
            Console.ResetColor();
        }        
    }


    /// <summary>
    /// Gets user input where the field can be optional. Allows skipping by pressing ENTER
    /// </summary>
    private static string? GetOptionalUserInput(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!.Trim();
        return string.IsNullOrWhiteSpace(input) ? null : input;
    }



    /// <summary>
    /// Prompts user to enter a valid date.
    /// </summary>
    private static DateTime GetDateInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                return date;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid date format. Please use YYYY-MM-DD.\n");
            Console.ResetColor();
        }
    }



    /// <summary>
    /// Prompts user for an optional date. Allows skipping by pressing ENTER.
    /// </summary>
    private static DateTime? GetNullableDateInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!.Trim();
            if (string.IsNullOrWhiteSpace(input)) return null;

            if (DateTime.TryParse(input, out DateTime date))
                return date;
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid date format. Please use YYYY-MM-DD.\n");
            Console.ResetColor();
        }
    }



    /// <summary>
    /// Allows the user to select a customer from a list.
    /// </summary>
    private static Customer? SelectCustomer(IEnumerable<Customer> customers)
    {
        while (true)
        {
            Console.WriteLine("\nSelect a customer for the project:");
            int index = 1;
            foreach (var customer in customers)
            {
                Console.WriteLine($"{index}. {customer.Name} ({customer.Email})");
                index++;
            }

            Console.Write("Choose customer (enter number): ");
            if (int.TryParse(Console.ReadLine(), out int customerIndex) && customerIndex >= 1 && customerIndex <= customers.Count())
            {
                return customers.ElementAt(customerIndex - 1);
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid selection. Please enter a valid number.\n");
            Console.ResetColor();
        }
    }



    /// <summary>
    /// Allows the user to select a project status.
    /// </summary>
    private static ProjectStatus? SelectProjectStatus()
    {
        var statuses = Enum.GetValues(typeof(ProjectStatus)).Cast<ProjectStatus>().ToList();
        
        while (true)
        {
            Console.WriteLine("\nSelect project status:");
            for (int i = 0; i < statuses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {statuses[i]}");
            }

            Console.Write("Choose status (enter number): ");
            if (int.TryParse(Console.ReadLine(), out int statusIndex) && statusIndex >= 1 && statusIndex <= statuses.Count)
            {
                return statuses[statusIndex - 1];
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid selection. Please enter a valid number.\n");
            Console.ResetColor();
        }
    }
    #endregion
}