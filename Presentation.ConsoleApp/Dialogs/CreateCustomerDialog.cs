using Business.Interfaces;
using Business.Models;

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

        // Hämta inmatning från användaren
        string name = GetUserInput("Enter Customer Name: ");
        string email = GetUserInput("Enter Customer Email: ");
        string phone = GetUserInput("Enter Customer Phone Number: ");

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

        Console.WriteLine("\nPress any key to return to the customer menu...");
        Console.ReadKey();
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Prompts user for input and ensures it's not empty.
    /// </summary>
    private static string GetUserInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!.Trim();
            if (!string.IsNullOrWhiteSpace(input)) return input;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("This field cannot be empty. Please enter a value.\n");
            Console.ResetColor();
        }
    }
    #endregion
}