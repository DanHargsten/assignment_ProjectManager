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
            Console.WriteLine("2. Handle Projects");
            Console.WriteLine("3. Exit");
            Console.Write("Pick an option: ");

            string option = Console.ReadLine()!;
            switch (option)
            {
                case "1":
                    await _customerMenu.ShowCustomerMenuAsync();
                    break;

                case "2":
                    await _projectMenu.ShowProjectMenuAsync();
                    break;

                case "3":
                    Console.Clear();
                    Console.WriteLine("Exiting application...");
                    exit = true;
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("Invalid input, press any key to try again...");
                    break;
            }
        }
    }
}