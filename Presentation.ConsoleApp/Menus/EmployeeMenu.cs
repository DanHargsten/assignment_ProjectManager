using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Dialogs.EmployeeDialogs;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Menus;

public class EmployeeMenu(
    CreateEmployeeDialog createEmployeeDialog,
    DeleteEmployeeDialog deleteEmployeeDialog,
    UpdateEmployeeDialog updateEmployeeDialog,
    ViewEmployeesDialog viewEmployeesDialog)
{
    private readonly CreateEmployeeDialog _createEmployeeDialog = createEmployeeDialog;
    private readonly DeleteEmployeeDialog _deleteEmployeeDialog = deleteEmployeeDialog;
    private readonly UpdateEmployeeDialog _updateEmployeeDialog = updateEmployeeDialog;
    private readonly ViewEmployeesDialog _viewEmployeesDialog = viewEmployeesDialog;

    public async Task ExecuteAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("            EMPLOYEES MANAGEMENT           ");
            Console.WriteLine("-------------------------------------------\n");
            Console.WriteLine("1. Add New Employee");
            Console.WriteLine("2. View All Employees");
            Console.WriteLine("3. Update an Employee");
            Console.WriteLine("4. Delete an Employee\n");

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
                    await _createEmployeeDialog.ExecuteAsync();
                    break;
                case "2":
                    await _viewEmployeesDialog.ExecuteAsync();
                    break;
                case "3":
                    await _updateEmployeeDialog.ExecuteAsync();
                    break;
                case "4":
                    await _deleteEmployeeDialog.ExecuteAsync();
                    return;
                default:
                    Console.WriteLine("\nInvalid selection. Press any key to try again...");
                    Console.ReadKey();
                    break;
            }
        }
    } 
}