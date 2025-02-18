using Presentation.ConsoleApp.Helpers;

namespace Presentation.ConsoleApp.Menus;

public class MainMenu(CustomerMenu customerMenu, ProjectMenu projectMenu)
{
    private readonly CustomerMenu _customerMenu = customerMenu;
    private readonly ProjectMenu _projectMenu = projectMenu;

    public async Task ShowMainMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("----------------------------------");
            Console.WriteLine("----- CONSOLE APPLICATION UI -----");
            Console.WriteLine("---------- CRUD testing ----------");
            Console.WriteLine("----------------------------------\n");
            Console.WriteLine("1. Handle Customers");
            Console.WriteLine("2. Handle Projects\n");

            ConsoleHelper.WriteLineColored("Press '0' or leave empty to Exit.", ConsoleColor.Yellow);

            Console.Write("> ");
            string option = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(option) || option == "0")
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

                default:
                    ConsoleHelper.WriteLineColored("\nInvalid input, press any key to try again...", ConsoleColor.Red);
                    Console.ReadKey();
                    break;
            }
        }
    }
}