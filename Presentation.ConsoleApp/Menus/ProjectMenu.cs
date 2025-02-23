using Presentation.ConsoleApp.Dialogs.ProjectDialogs;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Menus;


/// <summary>
/// Handles project-related menu options.
/// </summary>
public class ProjectMenu(CreateProjectDialog createProjectDialog, ViewProjectsDialog viewProjectsDialog, UpdateProjectDialog updateProjectDialog, DeleteProjectDialog deleteProjectDialog)
{
    private readonly CreateProjectDialog _createProjectDialog = createProjectDialog;
    private readonly ViewProjectsDialog _viewProjectsDialog = viewProjectsDialog;
    private readonly UpdateProjectDialog _updateProjectDialog = updateProjectDialog;
    private readonly DeleteProjectDialog _deleteProjectDialog = deleteProjectDialog;

    public async Task ShowProjectMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("             PROJECT MANAGEMENT            ");
            Console.WriteLine("-------------------------------------------\n");

            Console.WriteLine("1. Add New Project");
            Console.WriteLine("2. View All Projects");
            Console.WriteLine("3. Update a Project");
            Console.WriteLine("4. Delete a Project\n");

            ConsoleHelper.ShowExitPrompt("return to Main Menu");
            Console.Write("Select an option: ");
  
            string option = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(option))
            {
                return;
            }

            switch (option)
            {
                case "1":
                    await _createProjectDialog.ExecuteAsync();
                    break;
                case "2":
                    await _viewProjectsDialog.ExecuteAsync();
                    break;
                case "3":
                    await _updateProjectDialog.ExecuteAsync();
                    break;
                case "4":
                    await _deleteProjectDialog.ExecuteAsync();
                    break;

                default:
                    ConsoleHelper.WriteLineColored("\nInvalid input. Press any key to try again.", ConsoleColor.Red);
                    Console.ReadKey();
                    break;
            }
        }
    }
}