using Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class EmployeeEntity
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required, StringLength(50)]
    public string LastName { get; set; } = null!;

    [EmailAddress, MaxLength(150)]
    public string? Email { get; set; }

    [Phone, MaxLength(20)]
    public string? Phone { get; set; }

    [Required]
    public EmployeeRole Role { get; set; }


    public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; } = [];
}