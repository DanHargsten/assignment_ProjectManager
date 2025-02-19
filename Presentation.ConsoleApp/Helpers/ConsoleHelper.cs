namespace Presentation.ConsoleApp.Helpers;


/// <summary>
/// Provides helper methods for console output formatting.
/// </summary>
public class ConsoleHelper
{
    /// <summary>
    /// Writes text in a specified color and resets the color afterward.
    /// </summary>
    public static void WriteColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }


    /// <summary>
    /// Writes a full line in a specified color.
    /// </summary>
    public static void WriteLineColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }


    /// <summary>
    /// A notification to the user that a field with an asterisk is optional.
    /// </summary>
    public static void WriteOptionalFieldNotice()
    {
        WriteLineColored("* = optional input, press ENTER to skip\n", ConsoleColor.Yellow);
    }


    /// <summary>
    /// Displays a standard exit message with custom text.
    /// </summary>
    public static void ShowExitPrompt(string message)
    {
        Console.Write("Press ");
        WriteColored("'0' ", ConsoleColor.Yellow);
        Console.Write("or ");
        WriteColored("leave blank ", ConsoleColor.Yellow);
        Console.Write($"to {message}.\n> ");
    }
}