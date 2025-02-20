using Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public ProjectStatus Status { get; set; }

    

    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    public virtual CustomerEntity Customer { get; set; } = null!;

    public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; } = [];
}