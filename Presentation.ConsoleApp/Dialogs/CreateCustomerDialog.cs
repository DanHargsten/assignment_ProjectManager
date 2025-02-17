using Business.Interfaces;
using Business.Models;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs;

/// <summary>
/// Handles user input for creating a new customer and passes it to the CustomerService.
/// </summary>
public class CreateCustomerDialog(ICustomerService customerService)
{
    private readonly ICustomerService _customerService = customerService;

    #region Main Execution
    /// <summary>
    /// Handles user input to create a new customer.
    /// Uses CustomerService to perform the actual creation.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("------   CREATE NEW CUSTOMER   ------");
        Console.WriteLine("-------------------------------------\n");

        Console.WriteLine("* = optional");
        // Hämta inmatning från användaren
        string name = InputHelper.GetUserInput("Enter Customer Name: ");
        string? email = InputHelper.GetUserOptionalInput("* Enter Customer Email: ");
        string? phone = InputHelper.GetUserOptionalInput("* Enter Customer Phone Number: ");

        // Skapa formulär för att registrera kunden
        var form = new CustomerRegistrationForm
        {
            Name = name,
            Email = email,
            PhoneNumber = phone
        };

        // Skicka formuläret till CustomerService för att skapa kunden
        var success = await _customerService.CreateCustomerAsync(form);

        Console.Clear();
        Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;       
        Console.WriteLine(success ? "Customer created successfully!" : "Failed to create customer.");
        Console.ResetColor();

        Console.Write("\nPress any key to return to the ");
        ConsoleHelper.WriteColored("Customer Menu", ConsoleColor.Yellow);
        Console.WriteLine("...");
        Console.ReadKey();
    }
    #endregion
}