using Presentation.ConsoleApp.Dialogs;
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
            Console.WriteLine("Pick an option below");
            Console.WriteLine("1. Add New Project");
            Console.WriteLine("2. View All Projects");
            Console.WriteLine("3. Update a Project");
            Console.WriteLine("4. Delete a Project\n");

            ConsoleHelper.WriteLineColored("Press '0' or leave empty to go back to Main Menu.", ConsoleColor.Yellow);

            Console.Write("> ");
            string option = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(option) || option == "0")
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
            }
        }
    }
}