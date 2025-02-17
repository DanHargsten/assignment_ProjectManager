﻿using Presentation.ConsoleApp.Dialogs;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Menus;


/// <summary>
/// Handles customer-related menu options.
/// </summary>
public class CustomerMenu(CreateCustomerDialog createCustomerDialog, ViewCustomersDialog viewCustomersDialog, DeleteCustomerDialog deleteCustomerDialog)
{
    private readonly CreateCustomerDialog _createCustomerDialog = createCustomerDialog;
    private readonly ViewCustomersDialog _viewCustomersDialog = viewCustomersDialog;
    private readonly DeleteCustomerDialog _deleteCustomerDialog = deleteCustomerDialog;


    public async Task ShowCustomerMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("------     CUSTOMER MANAGMENT     ------");
            Console.WriteLine("----------------------------------------\n");
            Console.WriteLine("Pick an option below.");
            Console.WriteLine("1. Add New Customer");
            Console.WriteLine("2. View All Customers");
            Console.WriteLine("3. Update a Customer");
            Console.WriteLine("4. Delete a Customer\n");

            ConsoleHelper.WriteLineColored("Press '0' or leave empty to go back to Main Menu.", ConsoleColor.Yellow);

            Console.Write("> ");
            string option = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(option) || option == "0")
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
                //case "3":
                //    break;
                case "4":
                    await _deleteCustomerDialog.ExecuteAsync();
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Try again.");
                    Console.ResetColor();
                    break;
            }
        }
    }
}
