using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;
using System.Net.Http.Headers;

namespace Presentation.ConsoleApp.Dialogs;


/// <summary>
/// Handles the process of updating an existing project.
/// Lists all projects, allows the user to select one, and updates the details.
/// </summary>
public class UpdateProjectDialog(IProjectService projectService, IEmployeeService employeeService)
{
    private readonly IProjectService _projectService = projectService;
    private readonly IEmployeeService _employeeService = employeeService;


    /// <summary>
    /// Starts the update process by listing all available projects and prompting the user to select one.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("--------    UPDATE PROJECT    --------");
        Console.WriteLine("--------------------------------------\n");

        // Hämtar alla projekt från databasen
        var projects = (await _projectService.GetProjectsAsync()).ToList();
        if (projects.Count == 0)
        {
            Console.WriteLine("No projects found. Press any key to return...");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            // Skriver ut alla tillgängliga projekt i en numrerad lista
            Console.WriteLine("-------   Available projects   -------");
            int index = 1;
            foreach (var project in projects)
            {
                Console.WriteLine($"{index}. {project!.Title}");
                index++;
            }

            Console.Write("\nSelect a project by entering its number: ");
            string input = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(input)) return;

            // Säkerställer att användaren valt ett giltigt projektnummer
            if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= projects.Count)
            {
                var selectedProject = projects[selectedIndex - 1];
                await PromptForProjectUpdateAsync(selectedProject!);
                break;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please Enter a valid number.\n");
            Console.ResetColor();
        }
    }



    /// <summary>
    /// Prompts the user for new project details and updates the project.
    /// </summary>
    /// <param name="selectedProject">The project to be updated.</param>
    private async Task PromptForProjectUpdateAsync(Project selectedProject)
    {
        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("--------    UPDATE PROJECT    --------");
        Console.WriteLine("--------------------------------------\n");

        // Om beskrivningen är för lång, trunkera den så att den inte tar för mycket plats i terminalen
        string truncatedDescription = selectedProject.Description?.Length > 50
            ? selectedProject.Description[..50] + "..."
            : selectedProject.Description ?? "No description";

        // Visar nuvarande information om projektet
        Console.WriteLine($"Title: {selectedProject.Title}");
        Console.WriteLine($"Description: {truncatedDescription}");
        Console.WriteLine($"Start Date: {selectedProject.StartDate:yyyy-MM-dd}");
        Console.WriteLine($"End Date: {selectedProject.EndDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
        Console.WriteLine($"Current Status: {StatusHelper.GetFormattedStatus(selectedProject.Status)}");

        Console.WriteLine("\n-------------------------------------------");
        ConsoleHelper.WriteLineColored("\nLeave fields empty to keep current values\n", ConsoleColor.Yellow);

        // Hämtar användarens uppdateringar, behåller nuvarande värden om fälten lämnas tomma
        string newTitle = GetUserInput($"New Title: ", selectedProject.Title);
        string newDescription = GetUserInput($"New Description: ", selectedProject.Description ?? "");
        string newStartDateInput = GetValidDateInput($"New Start Date (yyyy-MM-dd): ", selectedProject.StartDate?.ToString("yyyy-MM-dd") ?? "");
        DateTime? newStartDate = string.IsNullOrWhiteSpace(newStartDateInput) ? selectedProject.StartDate : DateTime.Parse(newStartDateInput);

        string newEndDateInput = GetValidDateInput($"New End Date (yyyy-MM-dd):", selectedProject.EndDate?.ToString("yyyy-MM-dd") ?? "");
        DateTime? newEndDate = string.IsNullOrWhiteSpace(newEndDateInput) ? selectedProject.EndDate : DateTime.Parse(newEndDateInput);
        ProjectStatus newStatus = GetValidStatusInput($"New Status: ", selectedProject.Status);

        // Hantera anställda
        List<int>? newEmployeeIds = await SelectEmployeesForProjectAsync(selectedProject);

        // Uppdaterar projektet via ProjectService
        bool success = await _projectService.UpdateProjectAsync(selectedProject.Id, newTitle, newDescription, newStartDate, newEndDate, newStatus, newEmployeeIds);
        Console.Clear();

        if (success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Project updated successfully!");           
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to update project.");
        }

        Console.ResetColor();

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }





    /// <summary>
    /// Retrieves user input and returns the default value if the input is empty.
    /// </summary>
    private static string GetUserInput(string prompt, string defaultValue)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!;
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }



    /// <summary>
    /// Ensures the user provides a valid date input or keeps the current value.
    /// </summary>
    private static string GetValidDateInput(string prompt, string defaultValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(input)) return defaultValue;

            if (DateTime.TryParseExact(input, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
                return input;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nInvalid date format. Please use yyyy-MM-dd.");
            Console.ResetColor();
        }
    }



    /// <summary>
    /// Displays a list of project statuses and allows the user to select one.
    /// </summary>
    private static ProjectStatus GetValidStatusInput(string prompt, ProjectStatus currentStatus)
    {
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("Available project statuses");
        var statuses = Enum.GetValues<ProjectStatus>().Cast<ProjectStatus>().ToList();

        for (int i = 0; i < statuses.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {StatusHelper.GetFormattedStatus(statuses[i])}");
        }
        Console.WriteLine("--------------------------------------");

        while (true)
        {
            Console.Write("Choose status: ");
            string input = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(input))
                return currentStatus;

            if (int.TryParse(input, out int statusIndex) && statusIndex >= 1 && statusIndex <= statuses.Count)
            {
                return statuses[statusIndex - 1];
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nInvalid selection. Please enter a valid number.");
            Console.ResetColor();
        }
    }






    /// <summary>
    /// Prompts user to select employees for the project.
    /// </summary>
    /// <returns>Returns a list of selected employee IDs.</returns>
    private async Task<List<int>> SelectEmployeesForProjectAsync(Project selectedProject)
    {
        Console.Clear();
        Console.Write("\nDo you want to update the assigned employees? (Y/N): ");
        var input = Console.ReadLine()?.Trim().ToLower();
        if (input != "y") return selectedProject.EmployeeIds?.ToList() ?? [];

        // Hämta alla anställda
        var employees = (await _employeeService.GetEmployeesAsync()).ToList();
        if (employees.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No employees available.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to Project Menu");
            Console.ReadKey();
            return selectedProject.EmployeeIds?.ToList() ?? [];
        }

        var selectedEmployeeIds = selectedProject.EmployeeIds?.ToList() ?? [];

        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("       SELECT EMPLOYEES FOR PROJECT        ");
            Console.WriteLine("-------------------------------------------\n");

            // Visa alla anställda i en lista
            for (int i = 0; i < employees.Count; i++)
            {
                var employee = employees[i];
                if (employee == null) continue;

                string assigned = selectedEmployeeIds.Contains(employees[i]!.Id) ? "[SELECTED] " : "";
                Console.WriteLine($"{i + 1}. {assigned}{employee.FirstName} {employee.LastName} ({employee.Role})");
            }

            ConsoleHelper.ShowExitPrompt("finish selecting employees");

            string selection = Console.ReadLine()!.Trim();
            if (string.IsNullOrWhiteSpace(selection)) break;

            if (int.TryParse(selection, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= employees.Count)
            {
                int selectedId = employees[selectedIndex - 1]!.Id;
                if (selectedEmployeeIds.Contains(selectedId))
                    selectedEmployeeIds.Remove(selectedId);
                else
                    selectedEmployeeIds.Add(selectedId);
            }
            else
            {
                ConsoleHelper.WriteLineColored("Invalid selection.\n", ConsoleColor.Red);
            }
        }

        return selectedEmployeeIds;
    }
}