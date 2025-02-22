using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs;

public class CreateEmployeeDialog(IEmployeeService employeeService)
{
    private readonly IEmployeeService _employeeService = employeeService;


    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("             ADD NEW EMPLOYEE              ");
        Console.WriteLine("-------------------------------------------\n");

        // Användarinput
        string firstName = InputHelper.GetUserInput("Enter first name: ");
        string lastName = InputHelper.GetUserInput("Enter last name: ");
        string? email = InputHelper.GetUserOptionalInput("(optional) Enter email: ");
        string? phone = InputHelper.GetUserOptionalInput("(optional) Enter phone: ");

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
