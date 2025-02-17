using Business.Interfaces;
using Business.Models;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs;

/// <summary>
/// Handles viewing customers, including listing all customers and selecting one.
/// </summary>
public class ViewCustomersDialog(ICustomerService customerService, IProjectService projectService)
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IProjectService _projectService = projectService;

    #region Main Execution
    /// <summary>
    /// Shows the main customer viewing menu.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("--------    VIEW CUSTOMERS    --------");
        Console.WriteLine("--------------------------------------\n");

        var customers = (await _customerService.GetCustomersAsync()).ToList();
        if (!customers.Any())
        {
            Console.WriteLine("No customers found.");
            Console.ReadKey();
            return;
        }

        // Visa alla kunder i en lista
        Console.WriteLine("Available Customers:");
        for (int i = 0; i < customers.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {customers[i]!.Name} ({customers[i]!.Email})");
        }

        Console.Write("\nEnter customer number for details (or press ENTER to go back): ");
        string input = Console.ReadLine()!;
        if (string.IsNullOrWhiteSpace(input)) return;

        // Välj en kund
        if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= customers.Count)
        {
            var selectedCustomer = customers[selectedIndex - 1]!;
            await ViewCustomerDetailsAsync(selectedCustomer);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid selection.");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
    #endregion



    #region Helper Methods
    /// <summary>
    /// Displays detailed information about a selected customer, including active projects.
    /// </summary>
    private async Task ViewCustomerDetailsAsync(Customer customer)
    {
        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("---------  CUSTOMER DETAILS  ---------");
        Console.WriteLine("--------------------------------------\n");

        Console.WriteLine($"Name: {customer.Name}");
        Console.WriteLine($"Email: {customer.Email}");
        Console.WriteLine($"Phone: {customer.PhoneNumber}");

        // Hämta och visa kundens aktiva projekt
        var projects = (await _projectService.GetProjectsByCustomerIdAsync(customer.Id)).ToList();
        if (projects.Any())
        {
            Console.WriteLine("\nActive Projects:");
            foreach (var project in projects)
            {
                Console.WriteLine($"Title: {project.Title}\n Start Date:{project.StartDate:yyyy-MM-dd}\n Project Status:{StatusHelper.GetFormattedStatus(project.Status)}\n");
            }
        }
        else
        {
            Console.WriteLine("\nNo active projects.");
        }

        Console.WriteLine("\nPress any key to go back...");
        Console.ReadKey();
    }
    #endregion
}