using Data.Enums;

namespace Data.Entities;

public class ProjectEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }


    public ProjectStatus Status { get; set; }
    

    public int CustomerId { get; set; }
    public virtual CustomerEntity Customer { get; set; } = null!;

}