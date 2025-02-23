using Business.Interfaces;
using Business.Models;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs.CustomerDialogs;

/// <summary>
/// Handles user input for creating a new customer and passes it to the CustomerService.
/// </summary>
public class CreateCustomerDialog(ICustomerService customerService)
{
    private readonly ICustomerService _customerService = customerService;


    /// <summary>
    /// Guides the user through the process of creating a new customer.
    /// It collects input, validates it, and passes it to CustomerService for processing.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              CREATE CUSTOMER              ");
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("Fill in the fields below to add a new customer.\n");

        // Hämta användarinmatning
        string name = InputHelper.GetUserInput("Enter customer name: ");
        string? email = InputHelper.GetUserOptionalInput("(Optional) Enter customer email: ");
        string? phone = InputHelper.GetUserOptionalInput("(Optional) Enter customer phone number: ");

        // Visa en sammanfattning av den nya kunden
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("          REVIEW CUSTOMER DETAILS          ");
        Console.WriteLine("-------------------------------------------\n");

        Console.WriteLine($"Name:".PadRight(15) + $"{name}");
        Console.WriteLine($"Email:".PadRight(15) + $"{(!string.IsNullOrWhiteSpace(email) ? email : "No email provided")}");
        Console.WriteLine($"Phone:".PadRight(15) + $"{(!string.IsNullOrWhiteSpace(phone) ? phone : "No phone provided")}");

        Console.Write("\nAre the details correct? Press Y to confirm, or Enter to cancel: ");
        var confirmation = Console.ReadLine()?.Trim().ToLower();

        // Om användaren bekräftar skapas kunden
        if (confirmation == "y")
        {
            // Skapa registreringsformuläret
            var form = new CustomerRegistrationForm
            {
                Name = name,
                Email = email,
                PhoneNumber = phone
            };

            // Skicka kunden till service-lagret för att skapas
            bool success = await _customerService.CreateCustomerAsync(form);

            if (success)
                ConsoleHelper.WriteLineColored("\nCustomer created successfully!", ConsoleColor.Green);
            else
                ConsoleHelper.WriteLineColored("\nFailed to create customer.", ConsoleColor.Red);
        }
        else
        {
            ConsoleHelper.WriteLineColored("\nCustomer creation cancelled.", ConsoleColor.Yellow);
        }


        // Visa utgångsmeddelande
        ConsoleHelper.ShowExitPrompt("return to the Customer Menu");
        Console.ReadKey();
    }
}