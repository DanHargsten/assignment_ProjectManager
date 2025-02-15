﻿using Business.Interfaces;
using Business.Models;
using Data.Enums;

namespace Presentation.ConsoleApp.Dialogs;


/// <summary>
/// Handles the process of updating an existing project.
/// Lists all projects, allows the user to select one, and updates the details.
/// </summary>
public class UpdateProjectDialog(IProjectService projectService)
{
    private readonly IProjectService _projectService = projectService;


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
        Console.WriteLine($"Updating Project: {selectedProject.Title}");

        // Om beskrivningen är för lång, trunkera den så att den inte tar för mycket plats i terminalen
        string truncatedDescription = selectedProject.Description?.Length > 30
            ? selectedProject.Description[..30] + "..."
            : selectedProject.Description ?? "No description";

        // Visar nuvarande information om projektet
        Console.WriteLine($"Title: {selectedProject.Title}");
        Console.WriteLine($"Description: {truncatedDescription}");
        Console.WriteLine($"Start Date: {selectedProject.StartDate:yyyy-MM-dd}");
        Console.WriteLine($"End Date: {selectedProject.EndDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
        Console.WriteLine($"Current Status: {GetFormattedStatus(selectedProject.Status)}");

        Console.WriteLine("\nLeave fields empty to keep current values.\n");

        // Hämtar användarens uppdateringar, behåller nuvarande värden om fälten lämnas tomma
        string newTitle = GetUserInput($"New Title: ", selectedProject.Title);
        string newDescription = GetUserInput($"New Description: ", selectedProject.Description ?? "");
        string newStartDate = GetValidDateInput($"New Start Date (yyyy-MM-dd): ", selectedProject.StartDate.ToString("yyyy-MM-dd"));
        string newEndDate = GetValidDateInput($"New End Date (yyyy-MM-dd, current: ", selectedProject.EndDate?.ToString("yyyy-MM-dd") ?? "");
        ProjectStatus newStatus = GetValidStatusInput($"New Status: ", selectedProject.Status);

        // Uppdaterar projektet via ProjectService
        bool success = await _projectService.UpdateProjectAsync(selectedProject.Id, newTitle, newDescription, newStartDate, newEndDate, newStatus.ToString());
        Console.Clear();

        if (success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Project updated successfully!");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nFailed to update project.");
            Console.ResetColor();
        }

        Console.WriteLine("Press any key to return...");
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



    // Validering av datum-input genererat av ChatGPT.
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
            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
            Console.ResetColor();
        }
    }



    /// <summary>
    /// Displays a list of project statuses and allows the user to select one.
    /// </summary>
    private static ProjectStatus GetValidStatusInput(string prompt, ProjectStatus currentStatus)
    {
        Console.WriteLine("\nSelect project status:");
        var statuses = Enum.GetValues<ProjectStatus>().Cast<ProjectStatus>().ToList();

        for (int i = 0; i < statuses.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {GetFormattedStatus(statuses[i])}");
        }

        while (true)
        {
            Console.Write("Choose status (enter number, or leave empty to keep current): ");
            string input = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(input))
                return currentStatus;

            if (int.TryParse(input, out int statusIndex) && statusIndex >= 1 && statusIndex <= statuses.Count)
            {
                return statuses[statusIndex - 1];
            }

            Console.WriteLine("Invalid selection. Please enter a valid number.");
        }
    }



    // Enum formatering med switch genererad av ChatGPT
    /// <summary>
    /// Converts a ProjectStatus enum value to a user-friendly string.
    /// </summary>
    private static string GetFormattedStatus(ProjectStatus status)
    {
        return status switch
        {
            ProjectStatus.NotStarted => "Not Started",
            ProjectStatus.InProgress => "In Progress",
            _ => status.ToString()
        };
    }
}