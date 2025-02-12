using Data.Enums;

namespace Business.Models;

public class ProjectRegistrationForm
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
}