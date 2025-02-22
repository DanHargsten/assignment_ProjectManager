using Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class EmployeeEntity
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = null!;

    [EmailAddress, MaxLength(150)]
    public string? Email { get; set; }

    [Phone, MaxLength(20)]
    public string? Phone { get; set; }

    [Required]
    public EmployeeRole Role { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; } = [];
}