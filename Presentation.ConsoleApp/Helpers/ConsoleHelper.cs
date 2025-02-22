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
    /// Displays a standard exit message with custom text.
    /// </summary>
    public static void ShowExitPrompt(string message)
    {
        Console.Write("Press ");
        WriteColored("ENTER ", ConsoleColor.Yellow);        
        Console.Write($"(leave blank) to {message}.\n");
    }
}