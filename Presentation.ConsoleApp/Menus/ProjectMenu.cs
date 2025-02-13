using Business.Interfaces;
using Presentation.ConsoleApp.Dialogs;

namespace Presentation.ConsoleApp.Menus;

public class ProjectMenu(IProjectService ProjectService, ICustomerService customerService, ProjectDialog projectDialog)
{
    private readonly IProjectService _projectService = ProjectService;
    private readonly ICustomerService _customerService = customerService;
    private readonly ProjectDialog _projectDialog = projectDialog;

    public async Task ShowProjectMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("------        PROJECT MANAGMENT        ------");
            Console.WriteLine("---------------------------------------------\n");
            Console.WriteLine("Pick an option below");
            Console.WriteLine("1. Add New Project\n");
            Console.Write("Your option: ");

            string option = Console.ReadLine()!;
            switch (option)
            {
                case "1":
                    await _projectDialog.CreateProjectAsync();
                    break;
            }
        }
    }
}