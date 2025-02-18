using Business.Interfaces;
using Business.Models;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs;

/// <summary>
/// Handles updating an existing customer's details.
/// Lists all customers, allows the user to select one, and update the details.
/// </summary>
public class UpdateCustomerDialog(ICustomerService customerService)
{
    private readonly ICustomerService _customerService = customerService;

    /// <summary>
    /// Starts the update process by listing all customers and prompting the user to select one.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("--------   UPDATE CUSTOMER   --------");
        Console.WriteLine("--------------------------------------\n");

        // Hämtar alla kunder från databasen
        var customers = (await _customerService.GetCustomersAsync()).ToList();
        if (customers.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No customers found. Press any key to return...", ConsoleColor.Yellow);
            Console.ReadKey();
            return;
        }

        while (true)
        {
            // Skriver ut alla tillgängliga kunder i en numrerad lista
            Console.WriteLine("-------   Available customers   -------");
            for (int i = 0; i < customers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i]!.Name} ({customers[i]!.Email})");
            }

            Console.Write("\nSelect a customer by entering their number: ");
            string input = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(input)) return;

            // Säkerställer att användaren valt ett giltigt kundnummer
            if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= customers.Count)
            {
                var selectedCustomer = customers[selectedIndex - 1]!;
                await PromptForCustomerUpdateAsync(selectedCustomer);
                break;
            }

            ConsoleHelper.WriteLineColored("Invalid input. Please enter a valid number.\n", ConsoleColor.Red);
        }
    }

    /// <summary>
    /// Prompts the user for new customer details and updates the customer.
    /// </summary>
    /// <param name="selectedCustomer">The customer to be updated.</param>
    private async Task PromptForCustomerUpdateAsync(Customer selectedCustomer)
    {
        Console.Clear();
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("--------   UPDATE CUSTOMER   --------");
        Console.WriteLine("--------------------------------------\n");

        // Visar nuvarande information om kunden
        Console.WriteLine($"Name: {selectedCustomer.Name}");
        Console.WriteLine($"Email: {selectedCustomer.Email ?? "No email"}");
        Console.WriteLine($"Phone Number: {selectedCustomer.PhoneNumber ?? "No phone number"}");

        Console.WriteLine("\n** Leave fields empty to keep current values **");

        // Hämtar användarens uppdateringar, behåller nuvarande värden om fälten lämnas tomma
        string newName = GetUserInput("New Name: ", selectedCustomer.Name);
        string newEmail = GetOptionalUserInput("New Email: ", selectedCustomer.Email);
        string newPhone = GetOptionalUserInput("New Phone Number: ", selectedCustomer.PhoneNumber);

        // Uppdaterar kunden via CustomerService
        bool success = await _customerService.UpdateCustomerAsync(selectedCustomer.Id, newName, newEmail, newPhone);
        Console.Clear();

        if (success)
        {
            ConsoleHelper.WriteLineColored("Customer updated successfully!", ConsoleColor.Green);
        }
        else
        {
            ConsoleHelper.WriteLineColored("Failed to update customer.", ConsoleColor.Red);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

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
}
