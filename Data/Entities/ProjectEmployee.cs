namespace Data.Entities;

public class ProjectEmployee
{
    public int ProjectId { get; set; }
    public ProjectEntity Project { get; set; } = null!;

    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; } = null!;
}