using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Menus;

public class MainMenu(CustomerMenu customerMenu, ProjectMenu projectMenu, EmployeeMenu employeeMenu)
{
    private readonly CustomerMenu _customerMenu = customerMenu;
    private readonly ProjectMenu _projectMenu = projectMenu;
    private readonly EmployeeMenu _employeeMenu = employeeMenu;

    public async Task ShowMainMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("       PROJECT MANAGEMENT APPLICATION      ");
            Console.WriteLine("-------------------------------------------\n");

            Console.WriteLine("1. Handle Customers");
            Console.WriteLine("2. Handle Projects");
            Console.WriteLine("3. Handle Employees\n");

            ConsoleHelper.ShowExitPrompt("exit application");
            Console.Write("Select an option: ");

            string option = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(option))
            {
                Console.Clear();
                Console.WriteLine("Exiting application...");
                await Task.Delay(1500);
                return;
            }

            switch (option)
            {
                case "1":
                    await _customerMenu.ShowCustomerMenuAsync();
                    break;

                case "2":
                    await _projectMenu.ShowProjectMenuAsync();
                    break;

                case "3":
                    await _employeeMenu.ExecuteAsync();
                    break;

                default:
                    ConsoleHelper.WriteLineColored("\nInvalid input, press any key to try again.", ConsoleColor.Red);
                    Console.ReadKey();
                    break;
            }
        }
    }
}