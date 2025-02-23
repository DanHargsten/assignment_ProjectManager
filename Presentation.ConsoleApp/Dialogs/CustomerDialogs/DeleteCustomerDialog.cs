using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs.CustomerDialogs;

/// <summary>
/// Handles user input for deleting a customer and passes it to the CustomerService.
/// Ensures that all related projects are completed before deletion.
/// </summary>
public class DeleteCustomerDialog(ICustomerService customerService, IProjectService projectService)
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IProjectService _projectService = projectService;



    // ==================================================
    //                     MAIN EXECUTION
    // ==================================================

    /// <summary>
    /// Displays a list of customers and prompts the user to select one for deletion.
    /// Ensures all related projects are completed before deletion.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              DELETE CUSTOMER              ");
        Console.WriteLine("-------------------------------------------\n");

        // Hämtar alla kunder
        var customers = (await _customerService.GetCustomersAsync()).ToList();

        // Informera användaren om att inga kunder hittades
        if (customers.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No customers found.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to Main Menu");
            Console.ReadKey();
            return;
        }

        // Låt användaren välja en kund att ta bort
        var customer = SelectCustomer(customers);
        if (customer == null)       
            return;

        // Hämta kundens projekt och hantera aktiva projekt
        var projects = await _projectService.GetProjectsByCustomerIdAsync(customer.Id);
        await HandleActiveProjectsAsync(projects);

        // Bekräfta borttagning av kunden
        bool confirmed = ConfirmCustomerDeletion(customer, projects);
        if (!confirmed) return;

        // Försök ta bort kunden via CustomerService
        bool success = await _customerService.RemoveCustomerAsync(customer.Id);


        Console.Clear();
        if (success)
        {
            ConsoleHelper.WriteLineColored("Customer deleted successfully!", ConsoleColor.Green);
        }
        else
        {
            ConsoleHelper.WriteLineColored("Failed to delete customer.", ConsoleColor.Red);
        }

        ConsoleHelper.ShowExitPrompt("return to the Customer Menu");
        Console.ReadKey();
    }




    // ==================================================
    //                CUSTOMER SELECTION
    // ==================================================

    /// <summary>
    /// Displays a numbered list of customers and allows the user to select one to delete.
    /// </summary>
    /// <param name="customers">The list of available customers.</param>
    /// <returns>The selected customer, or null if an invalid selection was made.</returns>
    private static Customer? SelectCustomer(List<Customer?> customers)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("          SELECT CUSTOMER TO DELETE        ");
            Console.WriteLine("-------------------------------------------\n");
            
            // Skriv ut listan med kunder
            for (int i = 0; i < customers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i]!.Name}");
            }
            
            Console.Write("\nEnter customer number to delete: ");

            // Kontrollera att användaren valt ett giltigt index
            if (int.TryParse(Console.ReadLine(), out int selectedIndex) &&
                selectedIndex >= 1 && selectedIndex <= customers.Count)
            {
                return customers[selectedIndex - 1]!;
            }
            
            ConsoleHelper.WriteLineColored("Invalid selection.", ConsoleColor.Red);
        }
    }




    // ==================================================
    //            HANDLING ACTIVE PROJECTS
    // ==================================================

    /// <summary>
    /// Handles active or pending projects for the selected customer by allowing the user to mark them as completed.
    /// </summary>
    /// <param name="projects">The list of customer's projects.</param>
    private async Task HandleActiveProjectsAsync(IEnumerable<Project> projects)
    {
        var activeProjects = projects.Where(x => x.Status != ProjectStatus.Completed).ToList();

        while (activeProjects.Count > 0)
        {
            Console.Clear();
            ConsoleHelper.WriteLineColored("This customer has active or pending projects:", ConsoleColor.Yellow);


            // Lista alla pågående projekt
            for (int i = 0; i < activeProjects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {activeProjects[i].Title} ({StatusHelper.GetFormattedStatus(activeProjects[i].Status)})");
            }


            Console.Write("\nPick a project to mark as completed (or press Enter to cancel): ");
            string projectInput = Console.ReadLine()!.Trim();
            if (string.IsNullOrWhiteSpace(projectInput))            
                return;
            

            // Kontrollera att ett giltigt projektnummer angivits
            if (int.TryParse(projectInput, out int projectIndex) &&
                projectIndex >= 1 && projectIndex <= activeProjects.Count)
            {
                var selectedProject = activeProjects[projectIndex - 1];
                await MarkProjectAsCompletedAsync(selectedProject);

                // Uppdatera listan med aktiva projekt
                activeProjects = projects.Where(p => p.Status != ProjectStatus.Completed).ToList();
            }
            else
            {
                ConsoleHelper.WriteLineColored("Invalid selection.", ConsoleColor.Red);
            }
        }
    }




    // ==================================================
    //            MARK PROJECT AS COMPLETED
    // ==================================================

    /// <summary>
    /// Marks a selected project as completed.
    /// </summary>
    /// <param name="project">The project to be marked as completed.</param>
    private async Task MarkProjectAsCompletedAsync(Project project)
    {
        Console.Clear();
        Console.WriteLine($"Project:".PadRight(15) + $"{project.Title}");
        Console.WriteLine($"Description".PadRight(15) + $" {project.Description}");
        Console.WriteLine($"Start Date:".PadRight(15) + $" {project.StartDate:yyyy-MM-dd}");
        Console.WriteLine($"End Date:".PadRight(15) + $" {(project.EndDate.HasValue ? project.EndDate.Value.ToString("yyyy-MM-dd") : "Not specified")}");
        Console.WriteLine($"Status:".PadRight(15) + $" {StatusHelper.GetFormattedStatus(project.Status)}");

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
                ProjectStatus.Completed,
                project.EmployeeIds.ToList()
            );

            ConsoleHelper.WriteLineColored("\nProject marked as completed.", ConsoleColor.Green);
            Console.ReadKey();
        }
    }





    // ==================================================
    //        CUSTOMER DELETION CONFIRMATION
    // ==================================================

    /// <summary>
    /// Asks for final confirmation before deleting a customer.
    /// </summary>
    /// <param name="customer">The customer to be deleted.</param>
    /// <param name="projects">The customer's projects.</param>
    /// <returns>True if deletion is confirmed, otherwise false.</returns>
    private static bool ConfirmCustomerDeletion(Customer customer, IEnumerable<Project> projects)
    {
        if (projects.Any())
        {
            ConsoleHelper.WriteLineColored("All projects are completed.", ConsoleColor.Yellow);
        }

        Console.Write($"\nAre you sure you want to ");
        ConsoleHelper.WriteColored("delete", ConsoleColor.Red);
        Console.Write($" '{customer.Name}'? Press Y to confirm, or Enter to cancel: ");
        string confirmation = Console.ReadLine()!.Trim().ToLower();

        return confirmation == "yes";
    }
}