using Business.Interfaces;
using Business.Models;
using Data.Enums;
using Presentation.ConsoleApp.Dialogs;
using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Menus;

public class EmployeeMenu(IEmployeeService employeeService, CreateEmployeeDialog createEmployeeDialog, ViewEmployeesDialog viewEmployeesDialog)
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly CreateEmployeeDialog _createEmployeeDialog = createEmployeeDialog;
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
            Console.WriteLine("3. View Employee Details");

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
                    //await ViewEmployeeDetailsAsync();
                    //break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\nInvalid selection. Press any key to try again...");
                    Console.ReadKey();
                    break;
            }
        }
    } 
}