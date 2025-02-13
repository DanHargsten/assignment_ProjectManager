using Business.Interfaces;

namespace Presentation.ConsoleApp.Menus;

public class ProjectMenu(IProjectService ProjectService, ICustomerService customerService)
{
    private readonly IProjectService _projectService = ProjectService;
    private readonly ICustomerService _customerService = customerService;

    public async Task ShowProjectMenuAsync()
    {
        Console.WriteLine("YAY");
    }
}