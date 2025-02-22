using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Dialogs;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Menus;

public class EmployeeMenu(IEmployeeService employeeService, CreateEmployeeDialog createEmployeeDialog)
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly CreateEmployeeDialog _createEmployeeDialog = createEmployeeDialog;

    public async Task ExecuteAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("            EMPLOYEES MANAGEMENT           ");
            Console.WriteLine("-------------------------------------------\n");
            Console.WriteLine("1. Add New Employee");
            Console.WriteLine("2. View All Employees");
            Console.WriteLine("3. View Employee Details");

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
                    await _createEmployeeDialog.ExecuteAsync();
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




    
}
