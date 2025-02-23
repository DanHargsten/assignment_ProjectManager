using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;


namespace Presentation.ConsoleApp.Dialogs.ProjectDialogs;


/// <summary>
/// Handles the process of updating an existing project.
/// Lists all projects, allows the user to select one, and updates the details.
/// </summary>
public class UpdateProjectDialog(IProjectService projectService, IEmployeeService employeeService)
{
    private readonly IProjectService _projectService = projectService;
    private readonly IEmployeeService _employeeService = employeeService;





    // ==================================================
    //                   MAIN EXECUTION
    // ==================================================

    /// <summary>
    /// Starts the update process by listing all available projects and prompting the user to select one.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              UPDATE PROJECT               ");
        Console.WriteLine("-------------------------------------------\n");      

        // Hämtar alla projekt från databasen
        var projects = (await _projectService.GetProjectsAsync()).ToList();
        if (projects.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No projects found.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to the Project Menu");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            // Skriver ut alla tillgängliga projekt i en numrerad lista            
            int index = 1;
            foreach (var project in projects)
            {
                Console.WriteLine($"{index}. {project!.Title}");
                index++;
            }

            Console.Write("\nSelect a project by entering its number: ");
            ConsoleHelper.ShowExitPrompt("return to the Project Menu");

            string input = Console.ReadLine()!;

            // Om användaren lämnar fältet tomt, gå tillbaka
            if (string.IsNullOrWhiteSpace(input)) return;

            // Validerar att användaren har angett ett giltigt projektnummer
            if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= projects.Count)
            {
                var selectedProject = projects[selectedIndex - 1];

                await PromptForProjectUpdateAsync(selectedProject!);
                break;
            }

            ConsoleHelper.WriteLineColored("\nInvalid selection. Please enter a valid number.", ConsoleColor.Red);
        }
    }




    // ==================================================
    //              PROJECT UPDATE PROMPT
    // ==================================================

    /// <summary>
    /// Prompts the user for new project details and updates the project.
    /// </summary>
    /// <param name="selectedProject">The project to be updated.</param>
    private async Task PromptForProjectUpdateAsync(Project selectedProject)
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              UPDATE PROJECT               ");
        Console.WriteLine("-------------------------------------------\n");

        // Om beskrivningen är för lång, trunkera den så att den inte tar för mycket plats i terminalen
        string truncatedDescription = selectedProject.Description?.Length > 70
            ? selectedProject.Description[..70] + "..."
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

        // Uppdaterar projektet via ProjectService.
        bool success = await _projectService.UpdateProjectAsync(
            selectedProject.Id, 
            newTitle, 
            newDescription,
            newStartDate, 
            newEndDate, 
            newStatus,
            newEmployeeIds
        );
        
        Console.Clear();
        if (success)
        {
            ConsoleHelper.WriteLineColored("Project updated successfully!", ConsoleColor.Green);
        }
        else
        {
            ConsoleHelper.WriteLineColored("Failed to update project.", ConsoleColor.Red);
        }

        ConsoleHelper.ShowExitPrompt("return to Project Menu");
        Console.ReadKey();
    }




    // ==================================================
    //            SELECT EMPLOYEES FOR PROJECT
    // ==================================================

    /// <summary>
    /// Allows the user to update the assigned employees for a project.
    /// </summary>
    /// <param name="selectedProject">The project to update employees for.</param>
    /// <returns>Returns a list of selected employee IDs.</returns>
    private async Task<List<int>> SelectEmployeesForProjectAsync(Project selectedProject)
    {
        Console.Clear();
        Console.Write("\nDo you want to update the assigned employees? (Y/N): ");
        string? input = Console.ReadLine()?.Trim().ToLower();

        // Om användaren inte vill ändra, returnera nuvarande anställda
        if (input != "y") return selectedProject.EmployeeIds?.ToList() ?? [];

        // Hämta alla anställda från databasen
        var employees = (await _employeeService.GetEmployeesAsync()).ToList();

        // Om inga anställda finns, informera användaren och återgå
        if (employees.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No employees available.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to the Project Menu");
            Console.ReadKey();
            return selectedProject.EmployeeIds?.ToList() ?? [];
        }

        // Skapa en lista med nuvarande anställda som redan är kopplade till projektet
        var selectedEmployeeIds = selectedProject.EmployeeIds?.ToList() ?? [];

        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("       SELECT EMPLOYEES FOR PROJECT        ");
            Console.WriteLine("-------------------------------------------\n");

            // Skriv ut alla anställda i en numrerad lista
            for (int i = 0; i < employees.Count; i++)
            {
                var employee = employees[i];
                if (employee == null) continue;

                string assigned = selectedEmployeeIds.Contains(employees[i]!.Id) ? "[SELECTED] " : "";
                Console.WriteLine($"{i + 1}. {assigned}{employee.FirstName} {employee.LastName} ({employee.Role})");
            }

            ConsoleHelper.ShowExitPrompt("finish selecting employees");

            string selection = Console.ReadLine()!.Trim();

            // Om användaren lämnar fältet tomt, avsluta och returnera listan
            if (int.TryParse(selection, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= employees.Count)
            {
                int selectedId = employees[selectedIndex - 1]!.Id;

                // Om anställd redan är vald, ta bort den, annars lägg till den
                if (selectedEmployeeIds.Contains(selectedId))
                {
                    selectedEmployeeIds.Remove(selectedId);
                    ConsoleHelper.WriteLineColored($"Removed {employees[selectedIndex - 1]!.FirstName} from project.", ConsoleColor.Yellow);
                }
                else
                {
                    selectedEmployeeIds.Add(selectedId);
                    ConsoleHelper.WriteLineColored($"Added {employees[selectedIndex - 1]!.FirstName} to project.", ConsoleColor.Green);
                }
            }
            else
            {
                ConsoleHelper.WriteLineColored("Invalid selection. Please enter a valid number.\n", ConsoleColor.Red);
            }

            return selectedEmployeeIds;
        }
    }




    // ==================================================
    //                 HELPER METHODS
    // ==================================================

    /// <summary>
    /// Retrieves user input and returns the default value if the input is empty.
    /// </summary>
    /// <param name="prompt">The prompt message displayed to the user.</param>
    /// <param name="defaultValue">The current value to keep if the user leaves input empty.</param>
    /// <returns>Returns the user's input or the default value if left blank.</returns>
    private static string GetUserInput(string prompt, string defaultValue)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!;
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }



    /// <summary>
    /// Ensures the user provides a valid date input or keeps the current value.
    /// Accepts only the "yyyy-MM-dd" format.
    /// </summary>
    /// <param name="prompt">The message shown to the user.</param>
    /// <param name="defaultValue">The current date value if the user leaves input empty.</param>
    /// <returns>Returns the new date as a string, or the default value if left blank.</returns>
    private static string GetValidDateInput(string prompt, string defaultValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!;

            // Om användaren lämnar fältet tomt, behåll nuvarande värde
            if (string.IsNullOrWhiteSpace(input))
                return defaultValue;

            // Validera att datumet är korrekt formatterat
            if (DateTime.TryParseExact(input, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
                return input;

            ConsoleHelper.WriteLineColored("\nInvalid date format. Please use yyyy-MM-dd.", ConsoleColor.Red);
        }
    }



    /// <summary>
    /// Displays a list of project statuses and allows the user to select one.
    /// </summary>
    /// <param name="prompt">The prompt message displayed to the user.</param>
    /// <param name="currentStatus">The current status to keep if input is empty.</param>
    /// <returns>Returns the selected project status or keeps the current status.</returns>
    private static ProjectStatus GetValidStatusInput(string prompt, ProjectStatus currentStatus)
    {
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("         AVAILABLE PROJECT STATUSES        ");
        Console.WriteLine("-------------------------------------------");

        var statuses = Enum.GetValues<ProjectStatus>().Cast<ProjectStatus>().ToList();

        // Skriv ut alla tillgängliga statusalternativ
        for (int i = 0; i < statuses.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {StatusHelper.GetFormattedStatus(statuses[i])}");
        }
        Console.WriteLine("-------------------------------------------");

        while (true)
        {
            Console.Write("Choose status: ");
            string input = Console.ReadLine()!;

            // Om användaren lämnar fältet tomt, behåll nuvarande status
            if (string.IsNullOrWhiteSpace(input))
                return currentStatus;

            // Kontrollera att inmatningen är ett giltigt nummer i listan
            if (int.TryParse(input, out int statusIndex) && statusIndex >= 1 && statusIndex <= statuses.Count)
            {
                return statuses[statusIndex - 1];
            }

            ConsoleHelper.WriteLineColored("\nInvalid selection. Please enter a valid number.", ConsoleColor.Red);
        }
    }
}