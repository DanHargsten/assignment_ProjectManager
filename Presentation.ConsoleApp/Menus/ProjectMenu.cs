using Presentation.ConsoleApp.Dialogs;

namespace Presentation.ConsoleApp.Menus;

/// <summary>
/// Handles project-related menu options.
/// </summary>
public class ProjectMenu(CreateProjectDialog createProjectDialog, ViewProjectsDialog viewProjectsDialog)
{
    private readonly CreateProjectDialog _createProjectDialog = createProjectDialog;
    private readonly ViewProjectsDialog _viewProjectsDialog = viewProjectsDialog;

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
            Console.WriteLine("2. View All Projects\n");
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
            }
        }
    }
}