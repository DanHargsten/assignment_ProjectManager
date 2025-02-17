using Business.Interfaces;
namespace Presentation.ConsoleApp.Dialogs;



/// <summary>
/// Handles deleting a customer.
/// </summary>
public class DeleteCustomerDialog(ICustomerService customerService)
{
    private readonly ICustomerService _customerService = customerService;

    #region Main Execution
    /// <summary>
    /// Lists customers and prompts the user to delete one.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("--------   DELETE CUSTOMER   --------");
        Console.WriteLine("-------------------------------------\n");

        var customers = (await _customerService.GetCustomersAsync()).ToList();
        if (!customers.Any())
        {
            Console.WriteLine("No customers found.");
            Console.ReadKey();
            return;
        }

        // Lista kunder
        Console.WriteLine("Available Customers:");
        for (int i = 0; i < customers.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {customers[i]!.Name} ({customers[i]!.Email})");
        }

        Console.Write("\nEnter customer number to delete: ");
        if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= customers.Count)
        {
            var customer = customers[selectedIndex - 1]!;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nAre you sure you want to delete '{customer.Name}'?");
            Console.ResetColor();
            Console.Write("(yes/no): ");
            string confirmation = Console.ReadLine()!.Trim().ToLower();

            if (confirmation == "yes")
            {
                bool success = await _customerService.RemoveCustomerAsync(customer.Id);
               
                Console.Clear();
                Console.ForegroundColor = (success ? ConsoleColor.Green : ConsoleColor.Red);
                Console.WriteLine(success ? "Customer deleted successfully!" : "Failed to delete customer.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
    #endregion
}