using Presentation.ConsoleApp.Dialogs;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Menus;


/// <summary>
/// Handles customer-related menu options.
/// </summary>
public class CustomerMenu(CreateCustomerDialog createCustomerDialog, ViewCustomersDialog viewCustomersDialog, UpdateCustomerDialog updateCustomerDialog, DeleteCustomerDialog deleteCustomerDialog)
{
    private readonly CreateCustomerDialog _createCustomerDialog = createCustomerDialog;
    private readonly ViewCustomersDialog _viewCustomersDialog = viewCustomersDialog;
    private readonly UpdateCustomerDialog _updateCustomerDialog = updateCustomerDialog;
    private readonly DeleteCustomerDialog _deleteCustomerDialog = deleteCustomerDialog;


    public async Task ShowCustomerMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("            CUSTOMER MANAGEMENT            ");
            Console.WriteLine("-------------------------------------------\n");

            Console.WriteLine("1. Add New Customer");
            Console.WriteLine("2. View All Customers");
            Console.WriteLine("3. Update a Customer");
            Console.WriteLine("4. Delete a Customer\n");

            ConsoleHelper.ShowExitPrompt("return to Main Menu");
            Console.Write("Select an option: ");

            string option = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(option))
            {
                return;
            }

            switch (option)
            {
                case "1":
                    await _createCustomerDialog.ExecuteAsync();
                    break;
                case "2":
                    await _viewCustomersDialog.ExecuteAsync();
                    break;
                case "3":
                    await _updateCustomerDialog.ExecuteAsync();
                    break;
                case "4":
                    await _deleteCustomerDialog.ExecuteAsync();
                    break;

                default:
                    ConsoleHelper.WriteLineColored("\nInvalid input. Press any key to try again.", ConsoleColor.Red);  
                    Console.ReadKey();
                    break;
            }
        }
    }
}
