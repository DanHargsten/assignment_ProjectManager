using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs.EmployeeDialogs;



/// <summary>
/// Handles user input for creating a new employee and passes it to the EmployeeService.
/// </summary>
public class CreateEmployeeDialog(IEmployeeService employeeService)
{
    private readonly IEmployeeService _employeeService = employeeService;




    // ==================================================
    //                   MAIN EXECUTION
    // ==================================================

    /// <summary>
    /// Guides the user through the process of creating a new employee.
    /// It collects input, validates it, and passes it to EmployeeService for processing.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("             ADD NEW EMPLOYEE              ");
        Console.WriteLine("-------------------------------------------\n");
        Console.WriteLine("Fill in the fields below to add a new employee.\n");

        // Hämta användarinmatning
        string firstName = InputHelper.GetUserInput("Enter first name: ");
        string lastName = InputHelper.GetUserInput("Enter last name: ");
        string? email = InputHelper.GetUserOptionalInput("(Optional) Enter email: ");
        string? phone = InputHelper.GetUserOptionalInput("(Optional) Enter phone: ");

        // Låt användaren välja en roll
        var selectedRole = SelectEmployeeRole();
        if (selectedRole == null) return;


        // Visa en sammanfattning av den nya anställda
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("         REVIEW EMPLOYEE DETAILS          ");
        Console.WriteLine("-------------------------------------------\n");

        Console.WriteLine($"First Name:".PadRight(15) + $"{firstName}");
        Console.WriteLine($"Last Name:".PadRight(15) + $"{lastName}");
        Console.WriteLine($"Email:".PadRight(15) + $"{(!string.IsNullOrWhiteSpace(email) ? email : "No email provided")}");
        Console.WriteLine($"Phone:".PadRight(15) + $"{(!string.IsNullOrWhiteSpace(phone) ? phone : "No phone provided")}");
        Console.WriteLine($"Role:".PadRight(15) + $"{selectedRole}");

        Console.Write("\nAre the details correct? Press Y to confirm, or Enter to cancel: ");
        var confirmation = Console.ReadLine()?.Trim().ToLower();

        if (confirmation == "y")
        {
            // Skapa registreringsformuläret
            var form = new EmployeeRegistrationForm
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Role = selectedRole!.Value
            };
            
            // Skicka den nya anställda till service-lagret för att skapas
            bool success = await _employeeService.CreateEmployeeAsync(form);

            if (success)
                ConsoleHelper.WriteLineColored("\nEmployee added successfully!", ConsoleColor.Green);
            else
                ConsoleHelper.WriteLineColored("\nFailed to add employee.", ConsoleColor.Red);
        }
        else
        {
            ConsoleHelper.WriteLineColored("\nEmployee creation cancelled.", ConsoleColor.Yellow);
        }
        
        // Visa utgångsmeddelande
        ConsoleHelper.ShowExitPrompt("return to the Employee Menu");
        Console.ReadKey();
    }




    // ==================================================
    //                     HELPERS
    // ==================================================

    /// <summary>
    /// Allows the user to select an employee role from the available options.
    /// </summary>
    /// <returns>The selected EmployeeRole, or null if selection is invalid.</returns>
    private static EmployeeRole? SelectEmployeeRole()
    {
        Console.WriteLine("\nSelect a role");

        // Loopa igenom alla roller i EmployeeRole-enum och visa dem i listan
        foreach (var role in Enum.GetValues<EmployeeRole>())
        {
            Console.WriteLine($"{(int)role}. {role}");
        }

        Console.Write("\nEnter a number: ");

        bool validInput = int.TryParse(Console.ReadLine(), out int roleInput) && Enum.IsDefined(typeof(EmployeeRole), roleInput);

        if (!validInput)
        {
            Console.WriteLine("\nInvalid selection. Press any key to return.");
            Console.ReadKey();
            return null;
        }

        return (EmployeeRole)roleInput;
    }
}