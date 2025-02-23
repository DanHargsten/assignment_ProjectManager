using Business.Interfaces;
using Business.Models;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Dialogs.CustomerDialogs;


/// <summary>
/// Handles updating an existing customer's details.
/// Displays a list of customers, allows the user to select one, and updates the customer's details.
/// </summary>
public class UpdateCustomerDialog(ICustomerService customerService)
{
    private readonly ICustomerService _customerService = customerService;



    // ==================================================
    //                   MAIN EXECUTION
    // ==================================================

    /// <summary>
    /// Displays a list of customers and prompts the user to select one for updating.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              UPDATE CUSTOMER              ");
        Console.WriteLine("-------------------------------------------\n");

        // Hämtar alla kunder från databasen
        var customers = (await _customerService.GetCustomersAsync()).ToList();
        if (customers.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No customers found.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to Customer Menu");
            return;
        }

        while (true)
        {
            // Skriv ut en numrerad lista över tillgängliga kunder
            for (int i = 0; i < customers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i]!.Name}");
            }

            Console.Write("\nSelect a customer by entering their number: ");
            ConsoleHelper.ShowExitPrompt("return to Customer Menu");

            string input = Console.ReadLine()!;
           
            if (string.IsNullOrWhiteSpace(input)) return;

            // Hantera kundval om ett giltigt nummer anges
            if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= customers.Count)
            {
                var selectedCustomer = customers[selectedIndex - 1]!;

                await PromptForCustomerUpdateAsync(selectedCustomer);
                break;
            }

            ConsoleHelper.WriteLineColored("\nInvalid selection. Please enter a valid number.", ConsoleColor.Red);
            Console.WriteLine("Press any key to try again.");
            Console.ReadKey();
        }
    }




    // ==================================================
    //              PROMPT CUSTOMER UPDATE
    // ==================================================

    /// <summary>
    /// Prompts the user for new customer details and updates the customer.
    /// </summary>
    /// <param name="selectedCustomer">The customer to be updated.</param>
    private async Task PromptForCustomerUpdateAsync(Customer selectedCustomer)
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              UPDATE CUSTOMER              ");
        Console.WriteLine("-------------------------------------------\n");

        // Visar aktuell kundinfomration
        Console.WriteLine($"Name:           {selectedCustomer.Name}");
        Console.WriteLine($"Email:          {(!string.IsNullOrWhiteSpace(selectedCustomer.Email) ? selectedCustomer.Email : "No email provided")}");
        Console.WriteLine($"Phone Number:   {(!string.IsNullOrWhiteSpace(selectedCustomer.PhoneNumber) ? selectedCustomer.PhoneNumber : "No phone number provided")}");

        Console.WriteLine("\n-------------------------------------------");
        ConsoleHelper.WriteLineColored("\nLeave fields empty to keep current values\n", ConsoleColor.Yellow);


        // Hämta användarens nya värden – behåll aktuella värden om fälten lämnas tomma
        string newName = GetUserInput("New Name: ", selectedCustomer.Name);
        string newEmail = GetOptionalUserInput("New Email: ", selectedCustomer.Email);
        string newPhone = GetOptionalUserInput("New Phone Number: ", selectedCustomer.PhoneNumber);


        // Bekräftelse innan uppdatering
        Console.Write("\nDo you want to update this customer? Y to confirm, or press Enter to cancel: ");
        var confirmation = Console.ReadLine()?.Trim().ToLower();

        if (confirmation != "y")
        {
            ConsoleHelper.WriteLineColored("\nCustomer update cancelled.", ConsoleColor.Yellow);
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            return;
        }


        // Försök uppdatera kunden via CustomerService
        bool success = await _customerService.UpdateCustomerAsync(selectedCustomer.Id, newName, newEmail, newPhone);
        Console.Clear();

        if (success)
            ConsoleHelper.WriteLineColored("Customer updated successfully!", ConsoleColor.Green);
        else
            ConsoleHelper.WriteLineColored("Failed to update customer.", ConsoleColor.Red);

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }




    // ==================================================
    //                  HELPER METHODS
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
}