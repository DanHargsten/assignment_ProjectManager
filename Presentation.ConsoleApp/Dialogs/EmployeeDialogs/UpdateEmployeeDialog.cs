using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs.EmployeeDialogs;

/// <summary>
/// Handles updating an existing employee's details.
/// Displays a list of employees, allows the user to select one, and updates their details.
/// </summary>
public class UpdateEmployeeDialog(IEmployeeService employeeService)
{
    private readonly IEmployeeService _employeeService = employeeService;




    // ==================================================
    //                   MAIN EXECUTION
    // ==================================================

    /// <summary>
    /// Displays a list of employees and prompts the user to select one for updating.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              UPDATE EMPLOYEE              ");
        Console.WriteLine("-------------------------------------------\n");

        // Hämtar alla anställda från databasen
        var employees = (await _employeeService.GetEmployeesAsync()).ToList();
        if (employees.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No employees found.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to the Employee Menu");
            return;
        }

        while (true)
        {
            // Skriv ut en numrerad lista över tillgängliga anställda
            for (int i = 0; i < employees.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {employees[i]!.FirstName} {employees[i]!.LastName}");
            }

            Console.Write("\nSelect an employee by entering their number: ");
            ConsoleHelper.ShowExitPrompt("return to the Employee Menu");

            string input = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(input)) return;

            // Hantera anställdval om ett giltigt nummer anges
            if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= employees.Count)
            {
                var selectedEmployee = employees[selectedIndex - 1]!;

                await PromptForEmployeeUpdateAsync(selectedEmployee);
                break;
            }

            ConsoleHelper.WriteLineColored("\nInvalid selection. Please enter a valid number.", ConsoleColor.Red);
            Console.WriteLine("Press any key to try again.");
            Console.ReadKey();
        }
    }




    // ==================================================
    //          PROMPT EMPLOYEE UPDATE
    // ==================================================

    /// <summary>
    /// Prompts the user for new employee details and updates the employee.
    /// </summary>
    /// <param name="selectedEmployee">The employee to be updated.</param>
    private async Task PromptForEmployeeUpdateAsync(Employee selectedEmployee)
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              UPDATE EMPLOYEE              ");
        Console.WriteLine("-------------------------------------------\n");

        // Visar aktuell anställdinformation
        Console.WriteLine($"Name:".PadRight(20) + $"{selectedEmployee.FirstName} {selectedEmployee.LastName}");
        Console.WriteLine($"Email:".PadRight(20) + $"{(!string.IsNullOrWhiteSpace(selectedEmployee.Email) ? selectedEmployee.Email : "No email provided")}");
        Console.WriteLine($"Phone Number:".PadRight(20) + $"{(!string.IsNullOrWhiteSpace(selectedEmployee.Phone) ? selectedEmployee.Phone : "No phone number provided")}");
        Console.WriteLine($"Role:".PadRight(20) + $"{selectedEmployee.Role}");

        Console.WriteLine("\n-------------------------------------------");
        ConsoleHelper.WriteLineColored("\nLeave fields empty to keep current values\n", ConsoleColor.Yellow);

        // Hämta användarens nya värden – behåll aktuella värden om fälten lämnas tomma
        string newFirstName = GetUserInput("New First Name: ", selectedEmployee.FirstName);
        string newLastName = GetUserInput("New Last Name: ", selectedEmployee.LastName);
        string newEmail = GetOptionalUserInput("New Email: ", selectedEmployee.Email);
        string newPhone = GetOptionalUserInput("New Phone Number: ", selectedEmployee.Phone);
        
        // Hämta roll
        EmployeeRole newRole = GetValidRoleInput("New Role: ", selectedEmployee.Role);


        // Bekräftelse innan uppdatering
        Console.Write("\nDo you want to update this employee? Y to confirm, or press Enter to cancel: ");
        var confirmation = Console.ReadLine()?.Trim().ToLower();

        if (confirmation != "y")
        {
            ConsoleHelper.WriteLineColored("\nEmployee update cancelled.", ConsoleColor.Yellow);
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            return;
        }

        // Försök uppdatera den anställda via EmployeeService
        bool success = await _employeeService.UpdateEmployeeAsync(selectedEmployee.Id, newFirstName, newLastName, newEmail, newPhone, newRole);
        Console.Clear();

        if (success)
            ConsoleHelper.WriteLineColored("Employee updated successfully!", ConsoleColor.Green);
        else
            ConsoleHelper.WriteLineColored("Failed to update employee.", ConsoleColor.Red);

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    // ==================================================
    //               HELPER METHODS
    // ==================================================

    /// <summary>
    /// Retrieves user input and returns the default value if the input is empty.
    /// </summary>
    private static string GetUserInput(string prompt, string defaultValue)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!;
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }

    /// <summary>
    /// Retrieves optional user input, allowing an empty value.
    /// </summary>
    private static string GetOptionalUserInput(string prompt, string? defaultValue)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!;
        return string.IsNullOrWhiteSpace(input) ? defaultValue ?? "" : input;
    }


    /// <summary>
    /// Displays a list of employee roles and allows the user to select one.
    /// </summary>
    private static EmployeeRole GetValidRoleInput(string prompt, EmployeeRole currentRole)
    {
        Console.WriteLine("\n--------------------------------------");
        Console.WriteLine("Available employee roles:");

        var roles = Enum.GetValues<EmployeeRole>().Cast<EmployeeRole>().ToList();

        // Skriver ut rollerna
        for (int i = 0; i < roles.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {roles[i]}");
        }
        Console.WriteLine("--------------------------------------");

        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!;

            // Om tomt, behåll nuvarande roll
            if (string.IsNullOrWhiteSpace(input))
                return currentRole;

            // Försök konvertera input till enum
            if (int.TryParse(input, out int roleIndex) && roleIndex >= 1 && roleIndex <= roles.Count)
            {
                return roles[roleIndex - 1];
            }

            ConsoleHelper.WriteLineColored("Invalid selection. Please enter a valid number.\n", ConsoleColor.Red);
        }
    }
}
