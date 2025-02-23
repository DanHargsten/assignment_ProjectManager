using Business.Interfaces;
using Business.Models;
using Presentation.ConsoleApp.Dialogs.ProjectDialogs;
using Presentation.ConsoleApp.Helpers;


namespace Presentation.ConsoleApp.Dialogs.CustomerDialogs;

/// <summary>
/// Handles viewing customers, including listing all customers and selecting one.
/// </summary>
public class ViewCustomersDialog(ICustomerService customerService, IProjectService projectService)
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IProjectService _projectService = projectService;




    // ==================================================
    //                   MAIN EXECUTION
    // ==================================================

    /// <summary>
    /// Displays the customer list and allows selection for detailed view.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              VIEW CUSTOMERS               ");
        Console.WriteLine("-------------------------------------------\n");

        // Hämtar alla kunder från databasen
        var customers = (await _customerService.GetCustomersAsync()).ToList();
        if (customers.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No customers found", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to Customer Menu");
            Console.ReadKey();
            return;
        }


        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("              VIEW CUSTOMERS               ");
            Console.WriteLine("-------------------------------------------\n");
            Console.WriteLine("\nEnter a customer number to view details.");
            // Loopar genom alla kunder och visar dem i en lista
            for (int i = 0; i < customers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i]!.Name}");
            }

            Console.WriteLine("\nEnter a customer number to view details.");
            ConsoleHelper.ShowExitPrompt("return to Customer Menu");

            string input = Console.ReadLine()!;

            // Om användaren inte matar in något, returnera till meny
            if (string.IsNullOrWhiteSpace(input) || input == "0") return;

            // Validera användarens inmatning och hämta vald kund
            if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= customers.Count)
            {
                var selectedCustomer = customers[selectedIndex - 1]!;
                await ViewCustomerDetailsAsync(selectedCustomer);
                break;
            }
            else
            {
                ConsoleHelper.WriteLineColored("\nInvalid selection. Please enter a valid number.", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again.");
                Console.ReadKey();
            }
        }
    }




    // ==================================================
    //                   HELPER METHODS
    // ==================================================

    /// <summary>
    /// Displays detailed information about a selected customer, including active projects.
    /// </summary>
    private async Task ViewCustomerDetailsAsync(Customer customer)
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              CUSTOMER DETAILS             ");
        Console.WriteLine("-------------------------------------------\n");

        // Visar information om kunden
        Console.WriteLine($"ID:".PadRight(15) + $"{customer.Id}");
        Console.WriteLine($"Name:".PadRight(15) + $"{customer.Name}");
        Console.WriteLine($"Email:".PadRight(15) + $"{(!string.IsNullOrWhiteSpace(customer.Email) ? customer.Email : "No email provided")}");
        Console.WriteLine($"Phone:".PadRight(15) + $"{(!string.IsNullOrWhiteSpace(customer.PhoneNumber) ? customer.PhoneNumber : "No phone number provided")}");


        // Hämtar och visar projekt kopplade till kunden
        var projects = (await _projectService.GetProjectsByCustomerIdAsync(customer.Id)).ToList();

        if (projects.Count > 0)
        {
            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine("              ACTIVE PROJECTS              ");
            Console.WriteLine("-------------------------------------------\n");

            // Loopar genom projekten och visar information
            for (int i = 0; i < projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {projects[i].Title}".PadRight(32) + $"{StatusHelper.GetFormattedStatus(projects[i].Status)}");
            }

            Console.WriteLine("\n-------------------------------------------");


            // Loop för att visa projektlista och hantera val
            while (true)
            {
                Console.WriteLine("\nEnter a project number to view details.");
                ConsoleHelper.ShowExitPrompt("return to Customer Menu.");

                string input = Console.ReadLine()!.Trim();

                // Om användaren lämnar fältet tomt, gå tillbaka
                if (string.IsNullOrWhiteSpace(input))
                    return;

                // Validera och hämta det valda projektet
                if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= projects.Count)
                {
                    var selectedProject = projects[selectedIndex - 1];
                    await ViewProjectsDialog.ViewProjectDetailsAsync(selectedProject, "return to Customer Menu");
                    return;
                }
                else
                {
                    ConsoleHelper.WriteColored("Invalid selection. Please enter a valid project number.\n", ConsoleColor.Red);
                }
            }
        }
        else
        {
            Console.WriteLine("\nNo active projects.");
            Console.WriteLine("\n-------------------------------------------\n");
            ConsoleHelper.ShowExitPrompt("return to the Customer Menu");
            Console.ReadKey();
        }
    }
}