using Presentation.ConsoleApp.Dialogs;

namespace Presentation.ConsoleApp.Menus;

/// <summary>
/// Handles project-related menu options.
/// </summary>
public class ProjectMenu(CreateProjectDialog createProjectDialog, ViewProjectsDialog viewProjectsDialog, UpdateProjectDialog updateProjectDialog)
{
    private readonly CreateProjectDialog _createProjectDialog = createProjectDialog;
    private readonly ViewProjectsDialog _viewProjectsDialog = viewProjectsDialog;
    private readonly UpdateProjectDialog _updateProjectDialog = updateProjectDialog;

    public async Task ShowProjectMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("------        PROJECT MANAGMENT        ------");
            Console.WriteLine("---------------------------------------------\n");
            Console.WriteLine("Pick an option below");
            Console.WriteLine("1. Add New Project");
            Console.WriteLine("2. View All Projects");
            Console.WriteLine("3. Update a Project\n");
            Console.Write("Your option: ");

            string option = Console.ReadLine()!;
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
            }
        }
    }
}