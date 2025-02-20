using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Menus;

public class EmployeeMenu(IEmployeeService employeeService)
{
    private readonly IEmployeeService _employeeService = employeeService;

    public async Task ExecuteAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("              EMPLOYEE MANAGEMENT          ");
            Console.WriteLine("-------------------------------------------\n");
            Console.WriteLine("1. Add New Employee");
            Console.WriteLine("2. View All Employees");
            Console.WriteLine("3. View Employee Details");
            Console.WriteLine("0. Return to Main Menu");
            Console.Write("\nSelect an option: ");

            string? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    await AddEmployeeAsync();
                    break;
                case "2":
                    await ViewAllEmployeesAsync();
                    break;
                case "3":
                    await ViewEmployeeDetailsAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\nInvalid selection. Press any key to try again...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private async Task AddEmployeeAsync()
    {
        Console.Clear();
        Console.WriteLine("---- ADD NEW EMPLOYEE ----\n");
        ConsoleHelper.WriteOptionalFieldNotice();

        // Användarinput
        string firstName = InputHelper.GetUserInput("Enter first name: ");
        string lastName = InputHelper.GetUserInput("Enter last name: ");
        string? email = InputHelper.GetUserOptionalInput("(optional) Enter email: ");
        string? phone = InputHelper.GetUserOptionalInput("(optional) Enter phone: ");

        // Rollval (använder en separat metod för att strukturera koden bättre)
        var selectedRole = SelectEmployeeRole();
        if (selectedRole == null) return;

        // Skapa formuläret
        var form = new EmployeeRegistrationForm
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Role = selectedRole!.Value
        };

        bool success = await _employeeService.CreateEmployeeAsync(form);
        Console.WriteLine(success ? "\nEmployee added successfully!" : "\nFailed to add employee.");
        Console.ReadKey();
    }

    private async Task ViewAllEmployeesAsync()
    {
        Console.Clear();
        Console.WriteLine("---- EMPLOYEE LIST ----\n");

        var employees = (await _employeeService.GetEmployeesAsync()).ToList();
        if (!employees.Any())
        {
            Console.WriteLine("No employees found.");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < employees.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {employees[i]?.FirstName} {employees[i]?.LastName} - {employees[i]?.Role.ToString()}");
        }

        Console.WriteLine("\nPress any key to return...");
        Console.ReadKey();
    }

    private async Task ViewEmployeeDetailsAsync()
    {
        Console.Clear();
        Console.WriteLine("---- VIEW EMPLOYEE DETAILS ----\n");

        Console.Write("Enter Employee ID: ");
        bool isValidId = int.TryParse(Console.ReadLine(), out int id);
        if (!isValidId)
        {
            Console.WriteLine("Invalid ID format.");
            Console.ReadKey();
            return;
        }

        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            Console.WriteLine("Employee not found.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\n-------------------------------------------");
        Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}");
        Console.WriteLine($"Email: {employee.Email}");
        Console.WriteLine($"Phone: {employee.Phone}");
        Console.WriteLine($"Role: {employee.Role}");
        Console.WriteLine("-------------------------------------------\n");

        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }




    private EmployeeRole? SelectEmployeeRole()
    {
        Console.WriteLine("\nSelect a role:");
        foreach (var role in Enum.GetValues(typeof(EmployeeRole)))
        {
            Console.WriteLine($"{(int)role}. {role}");
        }

        Console.Write("\nEnter a number: ");
        bool validInput = int.TryParse(Console.ReadLine(), out int roleInput) && Enum.IsDefined(typeof(EmployeeRole), roleInput);

        if (!validInput)
        {
            Console.WriteLine("\nInvalid selection. Press any key to return...");
            Console.ReadKey();
            return null;
        }

        return (EmployeeRole)roleInput;
    }
}
