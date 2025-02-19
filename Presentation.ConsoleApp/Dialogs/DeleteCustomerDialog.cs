using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs;

/// <summary>
/// Handles deleting a customer.
/// Only allows deletion if all related projects are completed.
/// </summary>
public class DeleteCustomerDialog(ICustomerService customerService, IProjectService projectService)
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IProjectService _projectService = projectService;

    
    #region Main Execution

    /// <summary>
    /// Lists customers and prompts the user to delete one.
    /// Only allows deletion if all related projects are completed.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("--------   DELETE CUSTOMER   --------");
        Console.WriteLine("--------------------------------------\n");

        // Hämtar alla kunder
        var customers = (await _customerService.GetCustomersAsync()).ToList();

        // Om inga kunder finns, informera användaren
        if (customers.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No customers found.", ConsoleColor.Yellow);
            Console.Write("\nPress any key to return to the ");
            ConsoleHelper.WriteColored("Main Menu", ConsoleColor.Yellow);
            Console.WriteLine("...");
            Console.ReadKey();
            return;
        }

        // Välj en kund att radera
        var customer = SelectCustomer(customers);
        if (customer == null) return;

        // Hämtar alla projekt kopplade till kunden och hanterar aktiva projekt
        var projects = await _projectService.GetProjectsByCustomerIdAsync(customer.Id);
        await HandleActiveProjects(projects);

        // Bekräftar om användaren fortfarande vill ta bort kunden
        bool confirmed = ConfirmCustomerDeletion(customer, projects);
        if (!confirmed) return;

        bool success = await _customerService.RemoveCustomerAsync(customer.Id);


        Console.Clear();
        if (success)
        {
            ConsoleHelper.WriteLineColored("Customer deleted successfully!", ConsoleColor.Green);
        }
        else
        {
            ConsoleHelper.WriteLineColored("Failed to delete customer.", ConsoleColor.Green);
        } 

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    #endregion



    #region Helper Methods

    /// <summary>
    /// Displays a list of customers and allows the user to select one.
    /// </summary>
    /// <param name="customers"></param>
    /// <returns></returns>
    private static Customer? SelectCustomer(List<Customer?> customers)
    {
        // Skriver ut alla kunder
        for (int i = 0; i < customers.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {customers[i]!.Name}");
        }

        Console.Write("\nEnter customer number to delete: ");

        // Säkerställer att användaren valt ett giltigt kund-index
        if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= customers.Count)
        {
            return customers[selectedIndex - 1]!;
        }

        ConsoleHelper.WriteLineColored("Invalid selection.", ConsoleColor.Red);
        return null;
    }



    /// <summary>
    /// Ensures all active or pending projects are marked as completed before deleting a customer.
    /// </summary>
    private async Task HandleActiveProjects(IEnumerable<Project> projects)
    {
        var activeProjects = projects.Where(x => x.Status != ProjectStatus.Completed).ToList();

        while (activeProjects.Count > 0)
        {
            Console.Clear();
            ConsoleHelper.WriteLineColored("This customer has active or pending projects:", ConsoleColor.Yellow);

            // Skriver ut alla projekt som inte är slutförda
            for (int i = 0; i < activeProjects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {activeProjects[i].Title} ({StatusHelper.GetFormattedStatus(activeProjects[i].Status)})");
            }

            Console.Write("\nPick a project to mark as completed (or press ENTER to cancel): ");
            string projectInput = Console.ReadLine()!.Trim();
            if (string.IsNullOrWhiteSpace(projectInput)) return;

            // Säkerställer att användaren valt ett giltigt projekt-index
            if (int.TryParse(projectInput, out int projectIndex) && projectIndex >= 1 && projectIndex <= activeProjects.Count)
            {
                var selectedProject = activeProjects[projectIndex - 1];
                await MarkProjectAsCompleted(selectedProject);

                // Uppdaterar listan av aktiva projekt
                activeProjects = projects.Where(p => p.Status != ProjectStatus.Completed).ToList();
            }
            else
            {
                ConsoleHelper.WriteLineColored("Invalid selection.", ConsoleColor.Red);
            }
        }
    }



    /// <summary>
    /// Marks a project as completed.
    /// </summary>
    private async Task MarkProjectAsCompleted(Project project)
    {
        Console.Clear();
        Console.WriteLine($"Project: {project.Title}");
        Console.WriteLine($"Description: {project.Description}");
        Console.WriteLine($"Start Date: {project.StartDate:yyyy-MM-dd}");
        Console.WriteLine($"End Date: {(project.EndDate.HasValue ? project.EndDate.Value.ToString("yyyy-MM-dd") : "Not specified")}");
        Console.WriteLine($"Status: {StatusHelper.GetFormattedStatus(project.Status)}");

        Console.Write("\nDo you want to mark this project as completed? (yes/no): ");
        string confirm = Console.ReadLine()!.Trim().ToLower();
        if (confirm == "yes")
        {
            await _projectService.UpdateProjectAsync(
                project.Id,
                project.Title,
                project.Description,
                project.StartDate,
                project.EndDate,
                ProjectStatus.Completed
            );

            ConsoleHelper.WriteLineColored("\nProject marked as completed.", ConsoleColor.Green);
            Console.ReadKey();
        }
    }



    /// <summary>
    /// Asks for final confirmation before deleting a customer.
    /// </summary>
    private static bool ConfirmCustomerDeletion(Customer customer, IEnumerable<Project> projects)
    {
        if (projects.Any())
        {
            ConsoleHelper.WriteLineColored("All projects are completed.", ConsoleColor.Yellow);
        }

        Console.Write($"\nAre you sure you want to ");
        ConsoleHelper.WriteColored("delete", ConsoleColor.Red);
        Console.Write($" '{customer.Name}'? (yes/no): ");
        string confirmation = Console.ReadLine()!.Trim().ToLower();

        return confirmation == "yes";
    }

    #endregion
}