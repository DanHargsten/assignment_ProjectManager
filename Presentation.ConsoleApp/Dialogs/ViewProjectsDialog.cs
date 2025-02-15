using Business.Interfaces;
using Business.Models;

namespace Presentation.ConsoleApp.Dialogs;


/// <summary>
/// Handles viewing projects, including listing all projects, selecting one, and searching.
/// </summary>
public class ViewProjectsDialog(IProjectService projectService)
{
    private readonly IProjectService _projectService = projectService;

    #region Main Execution
    /// <summary>
    /// Shows the main project viewing menu.
    /// </summary>
    /// <remarks>
    /// This method is named ExecuteAsync() instead of ViewProjectAsync()
    /// to indicate that it is responsible for handling user interaction.
    /// </remarks>
    public async Task ExecuteAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("--------    VIEW PROJECTS    --------");
            Console.WriteLine("-------------------------------------\n");
            Console.WriteLine("1. View All Projects");
            Console.WriteLine("2. Search project by customer");
            Console.WriteLine("0. Back to main menu");
            Console.Write("\nPick an option: ");

            string option = Console.ReadLine()!;
            switch (option)
            {
                case "1":
                    await ViewAllProjectsAsync();
                    break;

                case "2":
                    await SearchProjectsByCustomerAsync();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }
    #endregion



    // --------------------------------



    #region Helper Methods
    /// <summary>
    /// Lists all projects with an index, allowing the user to select one for details.
    /// </summary>
    private async Task ViewAllProjectsAsync()
    {
        var projects = await _projectService.GetProjectsAsync();
        if (!projects.Any())
        {
            Console.WriteLine("No projects found.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine("------------------------------------");
        Console.WriteLine("--------    ALL PROJECTS    --------");
        Console.WriteLine("------------------------------------\n");
        int index = 1;
        foreach (var project in projects)
        {
            Console.WriteLine($"[{index}] {project!.Title} (Customer: {project.CustomerName})");
            index++;
        }

        Console.WriteLine("\nEnter project number for details");
        if (!int.TryParse(Console.ReadLine(), out int selectedIndex) || selectedIndex < 1 || selectedIndex > projects.Count())
        {
            Console.WriteLine("Invalid selection.");
            Console.ReadKey();
            return;
        }

        var selectedProject = projects.ElementAt(selectedIndex - 1)!;
        await ViewProjectDetailsAsync(selectedProject);
    }



    /// <summary>
    /// Displays detailed information about a selected project.
    /// </summary>
    private static Task ViewProjectDetailsAsync(Project project)
    {
        Console.Clear();
        Console.WriteLine("PROJECT DETAILS\n");
        Console.WriteLine($"Title: {project.Title}");
        Console.WriteLine($"Description: {project.Description}");
        Console.WriteLine($"Start Date: {project.StartDate:yyyy-MM-dd}");
        Console.WriteLine($"End Date: {(project.EndDate.HasValue ? project.EndDate.Value.ToString("yyyy-MM-dd") : "N/A")}");
        Console.WriteLine($"Status: {project.Status}");
        Console.WriteLine($"Customer: {project.CustomerName}");

        Console.WriteLine("\nPress any key to go back...");
        Console.ReadKey();

        return Task.CompletedTask;
    }




    /// <summary>
    /// Allows the user to search for projects by Customer ID, Name, or Email.
    /// </summary>
    private async Task SearchProjectsByCustomerAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("-------    SEARCH PROJECTS    -------");
        Console.WriteLine("---------    by customer    ---------");
        Console.WriteLine("-------------------------------------\n");

        Console.Write("Enter Customer ID, Name, or Email: ");
        string input = Console.ReadLine()!.Trim();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Invalid input. Please enter a valid Customer ID, Name, or Email.");
            Console.ReadKey();
            return;
        }

        IEnumerable<Project> projects;

        if (int.TryParse(input, out int customerID))
        {
            projects = await _projectService.GetProjectsByCustomerIdAsync(customerID);
        }
    }
    #endregion
}