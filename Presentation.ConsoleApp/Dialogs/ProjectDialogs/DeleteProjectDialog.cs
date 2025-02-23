using Business.Interfaces;

namespace Presentation.ConsoleApp.Dialogs.ProjectDialogs;

/// <summary>
/// Handles deleting a project.
/// Lists all projects, allows the user to select one, and confirms deletion.
/// </summary>
public class DeleteProjectDialog(IProjectService projectService)
{
    private readonly IProjectService _projectService = projectService;


    #region Main Execution
    /// <summary>
    /// Starts the delete process by listing all available projects and prompting the user to select one.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("--------    DELETE PROJECT    --------");
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
            for (int i = 0; i < projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {projects[i]!.Title}");
            }

            Console.Write("\nSelect a project to delete by entering its number: ");
            string input = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(input)) return;

            // Säkerställer att användaren valt ett giltigt projektnummer
            if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= projects.Count)
            {
                var selectedProject = projects[selectedIndex - 1]!;


                // Frågar efter bekräftelse innan radering
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nAre you sure you want to delete '{selectedProject.Title}'?");
                Console.ResetColor();
                Console.Write("(yes/no): ");

                string confirmation = Console.ReadLine()!.Trim().ToLower();
                if (confirmation == "yes")
                {
                    // Anropa ProjectService för att ta bort projektet
                    bool success = await _projectService.RemoveProjectAsync(selectedProject.Id);

                    Console.Clear();
                    if (success)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Project deleted successfully!");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed to delete project.");
                    }

                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Deletion cancelled.");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }


            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter a valid number.\n");
            Console.ResetColor();
        }
    }
    #endregion
}