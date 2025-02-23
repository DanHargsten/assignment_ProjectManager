using Business.Interfaces;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs.EmployeeDialogs;



/// <summary>
/// Handles viewing employees, including listing all employees and viewing details for a specific employee.
/// </summary>
public class ViewEmployeesDialog(IEmployeeService employeeService)
{
    private readonly IEmployeeService _employeeService = employeeService;



    // ==================================================
    //                   MAIN EXECUTION
    // ==================================================

    /// <summary>
    /// Displays the employee viewing menu, allowing the user to choose an action.
    /// </summary>
    public async Task ExecuteAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("              VIEW EMPLOYEES               ");
            Console.WriteLine("-------------------------------------------\n");
            Console.WriteLine("1. View All Employees");
            Console.WriteLine("2. View Employee Details by ID");
            Console.Write("\nPick an option: ");

            string option = Console.ReadLine()!;
            switch (option)
            {
                case "1":
                    await ViewAllEmployeesAsync();
                    break;

                case "2":
                    await ViewEmployeeDetailsAsync();
                    break;


                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }




    // ==================================================
    //                VIEW ALL EMPLOYEES
    // ==================================================

    /// <summary>
    /// Displays a list of all employees.
    /// </summary>
    private async Task ViewAllEmployeesAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              EMPLOYEE LIST                ");
        Console.WriteLine("-------------------------------------------\n");

        var employees = (await _employeeService.GetEmployeesAsync()).ToList();
        if (employees.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No employees found.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to Employee Menu");
            Console.ReadKey();
            return;
        }

        // Skriver ut alla anställda med indexnummer
        for (int i = 0; i < employees.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {employees[i]?.FirstName} {employees[i]?.LastName}".PadRight(30) + $"{employees[i]?.Role.ToString()}");
        }

        ConsoleHelper.ShowExitPrompt("return to Employee Menu");
        Console.ReadKey();
    }




    // ==================================================
    //              VIEW EMPLOYEE DETAILS
    // ==================================================

    /// <summary>
    /// Displays details for a specific employee.
    /// </summary>
    private async Task ViewEmployeeDetailsAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("           EMPLOYEE DETAILS                ");
        Console.WriteLine("-------------------------------------------\n");

        Console.Write("Enter Employee ID: ");
        string input = Console.ReadLine()!.Trim();

        // Validerar inmatning av ID
        if (!int.TryParse(input, out int employeeId))
        {
            ConsoleHelper.WriteLineColored("Invalid ID format.", ConsoleColor.Red);
            return;
        }

        // Hämtar anställd från tjänsten
        var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
        if (employee == null)
        {
            ConsoleHelper.WriteLineColored("Employee not found.", ConsoleColor.Red);
            return;
        }

        // Skriver ut detaljerad information om anställd
        Console.WriteLine("\n-------------------------------------------");
        Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}");
        Console.WriteLine($"Email: {employee.Email}");
        Console.WriteLine($"Phone: {employee.Phone}");
        Console.WriteLine($"Role: {employee.Role}");
        Console.WriteLine("-------------------------------------------\n");

        ConsoleHelper.ShowExitPrompt("return to the Employee Menu");

    }
}