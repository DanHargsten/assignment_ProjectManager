using Business.Interfaces;
using Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Presentation.ConsoleApp.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;

namespace Presentation.ConsoleApp.Dialogs;

/// <summary>
/// Handles viewing customers, including listing all customers and selecting one.
/// </summary>
public class ViewCustomersDialog(ICustomerService customerService, IProjectService projectService, ViewProjectsDialog viewProjectsDialog)
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IProjectService _projectService = projectService;
    private readonly ViewProjectsDialog _viewProjectsDialog = viewProjectsDialog;

    #region Main Execution
    /// <summary>
    /// Shows the main customer viewing menu.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              VIEW CUSTOMERS               ");
        Console.WriteLine("-------------------------------------------\n");

        var customers = (await _customerService.GetCustomersAsync()).ToList();
        if (customers.Count == 0)
        {
            ConsoleHelper.WriteLineColored("No customers found", ConsoleColor.Yellow);
            Console.WriteLine("\nPress any key to return to Customer Menu...");
            Console.ReadKey();
            return;
        }

        // Visa kundlista
        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("              VIEW CUSTOMERS               ");
            Console.WriteLine("-------------------------------------------\n");

            for (int i = 0; i < customers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i]!.Name} ({customers[i]!.Email})");
            }

            Console.WriteLine("\nEnter a customer number to view details.");
            ConsoleHelper.ShowExitPrompt("to Main Menu");

            string input = Console.ReadLine()!;

            // Avsluta med tomt eller 0
            if (string.IsNullOrWhiteSpace(input) || input == "0") return;

            // Välj en kund
            if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= customers.Count)
            {
                var selectedCustomer = customers[selectedIndex - 1]!;
                await ViewCustomerDetailsAsync(selectedCustomer);
                break;
            }
            else
            {
                ConsoleHelper.WriteLineColored("\nInvalid selection. Please try again.", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
    #endregion



    #region Helper Methods
    /// <summary>
    /// Displays detailed information about a selected customer, including active projects.
    /// </summary>
    private async Task ViewCustomerDetailsAsync(Customer customer)
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              CUSTOMER DETAILS             ");
        Console.WriteLine("-------------------------------------------\n");

        Console.WriteLine($"Name: ".PadRight(10) + $"{ customer.Name}");
        Console.WriteLine($"Email: ".PadRight(10) + $"{ customer.Email}");
        Console.WriteLine($"Phone: ".PadRight(10) + $"{ customer.PhoneNumber}");


        // Hämta och visa kundens aktiva projekt
        var projects = (await _projectService.GetProjectsByCustomerIdAsync(customer.Id)).ToList();

        if (projects.Count > 0)
        {
            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine("              Active Projects              ");
            Console.WriteLine("-------------------------------------------\n");

            int index = 1;
            foreach (var project in projects)
            {
                Console.WriteLine($"{index}. {project.Title}".PadRight(32) + $"{StatusHelper.GetFormattedStatus(project.Status)}");
                index++;
            }

            Console.WriteLine("\n-------------------------------------------");

            while (true)
            {
                Console.WriteLine("\nEnter a project number to view details.");
                ConsoleHelper.ShowExitPrompt("return to Customer Menu.");

                string input = Console.ReadLine()!.Trim();
                if (string.IsNullOrWhiteSpace(input) || input == "0")
                    return;

                if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= projects.Count)
                {
                    var selectedProject = projects[selectedIndex -1];
                    await ViewProjectsDialog.ViewProjectDetailsAsync(selectedProject, "return to Customer Menu");
                    return;
                }
                else
                {
                    ConsoleHelper.WriteColored("Invalid selection. Please enter a valid project number.\n", ConsoleColor.Red);
                }
            }
        }     
        else
        {
                Console.WriteLine("\nNo active projects.\n");
                ConsoleHelper.ShowExitPrompt("return to Customer Menu");
                Console.ReadKey();
        }
    }
    
    #endregion
}