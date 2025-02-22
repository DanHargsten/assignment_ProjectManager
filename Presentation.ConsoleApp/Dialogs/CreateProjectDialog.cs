﻿using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;


namespace Presentation.ConsoleApp.Dialogs;


/// <summary>
/// Handles user input for creating a new project and passes it to the ProjectService.
/// </summary>
public class CreateProjectDialog(IProjectService projectService, ICustomerService customerService, IEmployeeService employeeService)
{
    private readonly IProjectService _projectService = projectService;
    private readonly ICustomerService _customerService = customerService;
    private readonly IEmployeeService _employeeService = employeeService;


    #region Main Execution

    /// <summary>
    /// Handles user input to create a new project.
    /// Uses ProjectService to perform the actual creation.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              CREATE PROJECT               ");
        Console.WriteLine("-------------------------------------------\n");
        Console.WriteLine("Fill in the fields below to add a new project.\n");


        // Hämta projektdetaljer
        string title = InputHelper.GetUserInput("Enter project title: ");
        string? description = InputHelper.GetUserOptionalInput("(optional) Enter project description: ");
        DateTime? startDate = GetNullableDateInput("(optional) Enter project start date (YYYY-MM-DD): ");
        DateTime? endDate = GetNullableDateInput("(optional) Enter project end date (YYYY-MM-DD): ");


        // Välj kund
        var customers = (await _customerService.GetCustomersAsync()).ToList();
        if (customers.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No customers available. Please add a customer first.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to the Project Menu");
            return;
        }

        var selectedCustomer = SelectCustomer(customers!);
        if (selectedCustomer == null) return;

        // Välj projektstatus
        var selectedStatus = SelectProjectStatus();
        if (selectedStatus == null) return;


        // Skapa projektformulär
        var form = new ProjectRegistrationForm
        {
            Title = title,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            Status = selectedStatus.Value,
            CustomerId = selectedCustomer.Id,
            CreatedDate = DateTime.UtcNow
        };


        // Bekräfta skapandet av projektet
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("          CONFIRM PROJECT CREATION         ");
        Console.WriteLine("-------------------------------------------\n");
        Console.WriteLine($"Title: {form.Title}");
        Console.WriteLine($"Description: {form.Description}");
        Console.WriteLine($"Customer: {selectedCustomer.Name}");
        Console.WriteLine($"Start Date: {form.StartDate?.ToString("yyyy-MM-dd") ?? "Not specified"}");
        Console.WriteLine($"End Date: {form.EndDate?.ToString("yyyy-MM-dd") ?? "Not specified"}");
        Console.WriteLine($"Status: {StatusHelper.GetFormattedStatus(form.Status)}");

        Console.Write("\nAre the details correct? Press Y to confirm, or Enter to cancel: ");
        var confirmation = Console.ReadLine()?.Trim().ToLower();

        if (confirmation == "y")
        {
            // Skicka projektet till service-lagret för att skapas
            var success = await _projectService.CreateProjectAsync(form);
            
            if (success)
                ConsoleHelper.WriteLineColored("\nProject created successfully!", ConsoleColor.Green);
            else
                ConsoleHelper.WriteLineColored("\nFailed to create project.", ConsoleColor.Red);
        }
        else
        {
            ConsoleHelper.WriteLineColored("Project creation cancelled.", ConsoleColor.Yellow);
        }

        ConsoleHelper.ShowExitPrompt("continue");
        Console.ReadKey();

        await AssignEmployeesToProjectAsync(form);
    }

    #endregion





    #region Employee Assignment

    /// <summary>
    /// Prompts user to assign employees to the newly created project.
    /// </summary>
    private async Task AssignEmployeesToProjectAsync(ProjectRegistrationForm project)
    {
        Console.Clear();
        Console.Write("\nDo you want to assign employees to this project? (Y/N): ");
        var input = Console.ReadLine()?.Trim().ToLower();
        if (input != "y") return;

        
        // Hämta alla anställda
        var employees = (await _employeeService.GetEmployeesAsync()).ToList();
        if (employees.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No employees available to assign.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to Project Menu");
            Console.ReadKey();
            return;
        }

        var selectedEmployeeIds = new List<int>();

        while (true)
        {    
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("       SELECT EMPLOYEES FOR PROJECT        ");
            Console.WriteLine("-------------------------------------------\n");

            // Visa alla anställda i en lista
            for (int i = 0; i < employees.Count; i++)
            {
                string assigned = selectedEmployeeIds.Contains(employees[i]!.Id) ? "[SELECTED] " : "";
                Console.WriteLine($"{i + 1}. {assigned}{employees[i]!.FirstName} {employees[i]!.LastName} ({employees[i]!.Role})");
            }

            ConsoleHelper.ShowExitPrompt("finish assigning employees");

            string selection = Console.ReadLine()!.Trim();
            if (string.IsNullOrWhiteSpace(selection)) break;

            if (int.TryParse(selection, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= employees.Count)
            {
                int selectedId = employees[selectedIndex - 1]!.Id;
                if (selectedEmployeeIds.Contains(selectedId))
                    selectedEmployeeIds.Remove(selectedId);
                else
                    selectedEmployeeIds.Add(selectedId);
            }
            else
            {
                ConsoleHelper.WriteLineColored("Invalid selection.\n", ConsoleColor.Red);
            }
        }

        if (selectedEmployeeIds.Count != 0)
        {
            await _projectService.AssignEmployeesToProjectAsync(project.CustomerId, selectedEmployeeIds);
            ConsoleHelper.WriteLineColored("\nEmployees assigned successfully!", ConsoleColor.Green);
        }
    }




    private static List<int> ParseEmployeeSelection(string input, List<Employee> employees)
    {
        var selectedIds = new List<int>();

        var entries = input.Split(",");
        foreach (var entry in entries)
        {
            if (int.TryParse(entry.Trim(), out int index) && index >= 1 && index <= employees.Count)
            {
                selectedIds.Add(employees[index - 1].Id);
            }
            else
            {
                ConsoleHelper.WriteLineColored($"Invalid selection: {entry}. Skipping.", ConsoleColor.Red);
            }
        }

        return selectedIds;
    }


    #endregion



    #region Helper Methods

    /// <summary>
    /// Prompts user for an optional date. Allows skipping by pressing ENTER.
    /// </summary>
    private static DateTime? GetNullableDateInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!.Trim();
            if (string.IsNullOrWhiteSpace(input)) return null;

            if (DateTime.TryParse(input, out DateTime date))
                return date;

            ConsoleHelper.WriteLineColored("Invalid date format. Please use YYYY-MM-DD.\n", ConsoleColor.Red);
        }
    }



    /// <summary>
    /// Allows the user to select a customer from a list.
    /// </summary>
    private static Customer? SelectCustomer(IEnumerable<Customer> customers)
    {
        while (true)
        {
            // Visa alla tillgängliga kunder
            Console.WriteLine("\nSelect a customer for the project:");
            int index = 1;
            foreach (var customer in customers)
            {
                Console.WriteLine($"{index}. {customer.Name} ({customer.Email})");
                index++;
            }           

            ConsoleHelper.WriteLineColored("Enter a customer number to view details, or enter '0' (or leave blank) to return to Project Menu.", ConsoleColor.Yellow);
            Console.Write("> ");
            string input = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(input) || input == "0")
            {
                return null;
            }

            // Säkerställer att användaren valt ett giltigt kund-index
            if (int.TryParse(input, out int customerIndex) && customerIndex >= 1 && customerIndex <= customers.Count())
            {
                return customers.ElementAt(customerIndex - 1);
            }

            ConsoleHelper.WriteLineColored("Invalid selection. Please enter a valid number.\n", ConsoleColor.Red);
        }
    }



    /// <summary>
    /// Allows the user to select a project status.
    /// </summary>
    private static ProjectStatus? SelectProjectStatus()
    {
        var statuses = Enum.GetValues<ProjectStatus>().Cast<ProjectStatus>().ToList();
        
        while (true)
        {
            // Lista tillgängliga statusar
            Console.WriteLine("\nSelect project status:");
            for (int i = 0; i < statuses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {StatusHelper.GetFormattedStatus(statuses[i])}");
            }

            Console.Write("Choose status (enter number): ");
            if (int.TryParse(Console.ReadLine(), out int statusIndex) && statusIndex >= 1 && statusIndex <= statuses.Count)
            {
                return statuses[statusIndex - 1];
            }

            ConsoleHelper.WriteLineColored("Invalid selection. Please enter a valid number.\n", ConsoleColor.Red);
        }
    }





    #endregion
}