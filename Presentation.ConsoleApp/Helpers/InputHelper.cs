namespace Presentation.ConsoleApp.Helpers;


/// <summary>
/// Provides helper methods for handling user input in the console.
/// </summary>
public static class InputHelper
{
    /// <summary>
    /// Gets user input and ensures it is not empty.
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public static string GetUserInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!.Trim();
            if (!string.IsNullOrWhiteSpace(input)) return input;

            ConsoleHelper.WriteLineColored("This field cannot be empty. Please enter a value.\n", ConsoleColor.Red);
        }
    }


    /// <summary>
    /// Gets user input, but allows an empty value.
    /// Returns null if the input is empty.
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public static string? GetUserOptionalInput(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!.Trim();
        return string.IsNullOrWhiteSpace(input) ? null : input;
    }
}