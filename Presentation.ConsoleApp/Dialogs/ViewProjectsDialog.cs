using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;
using System.ComponentModel.Design;

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
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("             VIEW ALL PROJECTS             ");
        Console.WriteLine("-------------------------------------------\n");
        int index = 1;
        foreach (var project in projects)
        {
            Console.WriteLine($"{index}. {project!.Title}");
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
        await ViewProjectDetailsAsync(selectedProject, "return to View Projects Menu");
    }



    /// <summary>
    /// Displays detailed information about a selected project.
    /// </summary>
    public static Task ViewProjectDetailsAsync(Project project, string exitMessage)
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              PROJECT DETAILS              ");
        Console.WriteLine("-------------------------------------------\n");

        Console.WriteLine($"ID:".PadRight(18) + $"{project.Id}");
        Console.WriteLine($"Title:".PadRight(18) + $"{project.Title}");
        Console.WriteLine($"Description:".PadRight(18) + $"{project.Description}");
        Console.WriteLine($"Customer:".PadRight(18) + $"{project.CustomerName}");
        Console.WriteLine($"Start Date:".PadRight(18) + $"{(project.StartDate.HasValue ? project.StartDate.Value.ToString("yyyy-MM-dd") : "Not specified.")}");
        Console.WriteLine($"End Date:".PadRight(18) + $"{(project.EndDate.HasValue ? project.EndDate.Value.ToString("yyyy-MM-dd") : "Not specified.")}");
        Console.WriteLine($"Status:".PadRight(18) + $"{StatusHelper.GetFormattedStatus(project.Status)}\n");
        Console.WriteLine($"Created:".PadRight(18) + $"{project.CreatedDate:yyyy-MM-dd}");

        Console.WriteLine("\n-------------------------------------------");

        ConsoleHelper.ShowExitPrompt(exitMessage);
        Console.ReadKey();

        return Task.CompletedTask;
    }




    /// <summary>
    /// Allows the user to search for projects by Customer ID, Name, or Email.
    /// </summary>
    private async Task SearchProjectsByCustomerAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              SEARCH PROJECTS              ");
        Console.WriteLine("-------------------------------------------\n");     

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
        else
        {
            projects = await _projectService.GetProjectsByCustomerNameOrEmailAsync(input);
        }
        
        if (!projects.Any())
        {
            Console.WriteLine("\nNo projects found.");
        }
        else
        {
            Console.WriteLine("\nFound projects:");
            foreach (var project in projects)
            {
                Console.WriteLine($"- {project.Title} ({StatusHelper.GetFormattedStatus(project.Status)})");
            }
        }

        Console.WriteLine("\nPress any key to return...");
        Console.ReadKey();
    }
    #endregion
}