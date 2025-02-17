using Data.Enums;

namespace Presentation.ConsoleApp.Helpers;

/// <summary>
/// Provides helper methods for formatting project status values.
/// </summary>
public static class StatusHelper
{
    /// <summary>
    /// Converts a ProjectStatus enum value to a user-friendly string.
    /// </summary>
    public static string GetFormattedStatus(ProjectStatus status)
    {
        return status switch
        {
            ProjectStatus.NotStarted => "Not Started",
            ProjectStatus.InProgress => "In Progress",
            _ => status.ToString()
        };
    }
}