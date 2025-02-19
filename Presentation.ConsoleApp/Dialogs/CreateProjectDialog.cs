using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;


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
        ConsoleHelper.WriteOptionalFieldNotice();


        // Få användarens input för projektdetaljer
        string title = InputHelper.GetUserInput("Enter project title: ");
        string? description = InputHelper.GetUserOptionalInput("(optional) Enter project description: ");

        // Start- och slutdatum från användaren
        DateTime? startDate = GetNullableDateInput("(optional) Enter project start date (YYYY-MM-DD): ");
        DateTime? endDate = GetNullableDateInput("(optional) Enter project end date (YYYY-MM-DD): ");


        // Lista befintliga kunder och låt användaren välja en
        var customers = (await _customerService.GetCustomersAsync()).ToList();
        if (customers.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No customers available. Please add a customer first.", ConsoleColor.Yellow);
            Console.WriteLine("\nPress any key to return...");
            return;
        }

        var selectedCustomer = SelectCustomer(customers!);
        if (selectedCustomer == null) return;

        // Lista tillgängliga projektstatusar och låt användaren välja en
        var selectedStatus = SelectProjectStatus();
        if (selectedStatus == null) return;


        // Skapa ett formulär med all information
        var form = new ProjectRegistrationForm
        {
            Title = title,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            Status = selectedStatus.Value,
            CustomerId = selectedCustomer.Id,
            CreatedDate = DateTime.UtcNow
        };


        // Visa bekräftelse innan projektet skapas
        Console.Clear();
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("-----   Confirm Project Creation   -----");
        Console.WriteLine("----------------------------------------\n");

        Console.WriteLine("--- Project details");
        Console.WriteLine($"Title: {form.Title}");
        Console.WriteLine($"Description: {form.Description}");
        Console.WriteLine($"Customer: {selectedCustomer.Name}");
        Console.WriteLine($"Start Date: {form.StartDate?.ToString("yyyy-MM-dd") ?? "Not specified"}");
        Console.WriteLine($"End Date: {form.EndDate?.ToString("yyyy-MM-dd") ?? "Not specified"}");
        Console.WriteLine($"Status: {StatusHelper.GetFormattedStatus(form.Status)}");
        Console.WriteLine($"Created Date: {form.CreatedDate:yyyy-MM-dd}");

        Console.Write("\nDo you want to create this project? Y to accept, or press Enter to cancel: ");
        var confirmation = Console.ReadLine()?.Trim().ToLower();

        if (confirmation == "y")
        {
            // Skicka projektet till service-lagret för att skapas
            var success = await _projectService.CreateProjectAsync(form);
            
            if (success)
                ConsoleHelper.WriteLineColored("\nProject created successfully!", ConsoleColor.Green);
            else
                ConsoleHelper.WriteLineColored("\nFailed to create project.", ConsoleColor.Red);
        }
        else
        {
            ConsoleHelper.WriteLineColored("Project creation cancelled.", ConsoleColor.Yellow);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    #endregion



    #region Helper Methods

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

            ConsoleHelper.WriteLineColored("Invalid date format. Please use YYYY-MM-DD.\n", ConsoleColor.Red);
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

            ConsoleHelper.WriteLineColored("Invalid date format. Please use YYYY-MM-DD.\n", ConsoleColor.Red);
        }
    }



    /// <summary>
    /// Allows the user to select a customer from a list.
    /// </summary>
    private static Customer? SelectCustomer(IEnumerable<Customer> customers)
    {
        while (true)
        {
            // Visa alla tillgängliga kunder
            Console.WriteLine("\nSelect a customer for the project:");
            int index = 1;
            foreach (var customer in customers)
            {
                Console.WriteLine($"{index}. {customer.Name} ({customer.Email})");
                index++;
            }           

            ConsoleHelper.WriteLineColored("Enter a customer number to view details, or enter '0' (or leave blank) to return to Project Menu.", ConsoleColor.Yellow);
            Console.Write("> ");
            string input = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(input) || input == "0")
            {
                return null;
            }

            // Säkerställer att användaren valt ett giltigt kund-index
            if (int.TryParse(input, out int customerIndex) && customerIndex >= 1 && customerIndex <= customers.Count())
            {
                return customers.ElementAt(customerIndex - 1);
            }

            ConsoleHelper.WriteLineColored("Invalid selection. Please enter a valid number.\n", ConsoleColor.Red);
        }
    }



    /// <summary>
    /// Allows the user to select a project status.
    /// </summary>
    private static ProjectStatus? SelectProjectStatus()
    {
        var statuses = Enum.GetValues<ProjectStatus>().Cast<ProjectStatus>().ToList();
        
        while (true)
        {
            // Lista tillgängliga statusar
            Console.WriteLine("\nSelect project status:");
            for (int i = 0; i < statuses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {StatusHelper.GetFormattedStatus(statuses[i])}");
            }

            Console.Write("Choose status (enter number): ");
            if (int.TryParse(Console.ReadLine(), out int statusIndex) && statusIndex >= 1 && statusIndex <= statuses.Count)
            {
                return statuses[statusIndex - 1];
            }

            ConsoleHelper.WriteLineColored("Invalid selection. Please enter a valid number.\n", ConsoleColor.Red);
        }
    }

    #endregion
}