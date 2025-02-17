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
    /// The asterisk is colored for visual contrast
    /// </summary>
    public static void WriteOptionalFieldNotice()
    {
        WriteColored(" *", ConsoleColor.Yellow);
        Console.WriteLine(" = optional input, press ENTER to skip\n");
    }
}