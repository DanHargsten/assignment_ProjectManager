using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Data.Interfaces;

namespace Presentation.ConsoleApp.Dialogs;

public class ProjectDialog(IProjectService ProjectService, ICustomerService customerService, IProjectRepository projectRepository)
{
    private readonly IProjectService _projectService = ProjectService;
    private readonly ICustomerService _customerService = customerService;
    private readonly IProjectRepository _projectRepository = projectRepository;


    public async Task CreateProjectAsync()
    {
        Console.Clear();
        Console.WriteLine("CREATE NEW PROJECT\n");

        // --- Steg 1: Hämta användarinmatning ---
        Console.Write("Enter project title: ");
        string title = Console.ReadLine()!;

        Console.Write("Enter project description (optional): ");
        string? description = Console.ReadLine();

        Console.Write("Enter start date (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
        {
            Console.WriteLine("Invalid date format. Please use YYYY-MM-DD.");
            return;
        }

        Console.Write("Enter end date (YYYY-MM-DD) or leave blank if unknown: ");
        string endDateInput = Console.ReadLine()!;
        DateTime? endDate = string.IsNullOrWhiteSpace(endDateInput) ? null : DateTime.Parse(endDateInput);

        // --- Steg 2: Hämta lista med kunder och låt användaren välja ---
        var customers = await _customerService.GetCustomersAsync();
        if (!customers.Any())
        {
            Console.WriteLine("No customers available. Please add a customer first.");
            return;
        }

        Console.WriteLine("\nSelect a customer:");
        int index = 1;
        foreach (var customer in customers)
        {
            Console.WriteLine($"{index}. {customer!.Name} ({customer.Email})");
            index++;
        }

        Console.Write("Choose customer (enter number): ");
        if (!int.TryParse(Console.ReadLine(), out int customerIndex) || customerIndex < 1 || customerIndex > customers.Count())
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var selectedCustomer = customers.ElementAt(customerIndex - 1)!;

        // --- Steg 3: Välj status för projektet ---
        Console.WriteLine("\nSelect project status:");
        var statuses = Enum.GetValues(typeof(ProjectStatus)).Cast<ProjectStatus>().ToList();
        for (int i = 0; i < statuses.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {statuses[i]}");
        }

        Console.Write("Choose status (enter number): ");
        if (!int.TryParse(Console.ReadLine(), out int statusIndex) || statusIndex < 1 || statusIndex > statuses.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var selectedStatus = statuses[statusIndex - 1];

        // <<<<<<<<<<<<<<<<<< DEBUG >>>>>>>>>>>>>>>>> //
        Console.WriteLine($"DEBUG: Selected customer ID: {selectedCustomer.Id}");
        /////////////////////////////////


        // --- Steg 4: Skapa projektet via `ProjectService` ---
        var success = await _projectService.CreateProjectAsync(new ProjectRegistrationForm
        {
            Title = title,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            Status = selectedStatus,
            CustomerId = selectedCustomer.Id
        });


        // --- Steg 5: Bekräftelse ---
        Console.WriteLine(success ? "Project created successfully!" : "Failed to create project.");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}