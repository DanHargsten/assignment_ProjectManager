using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class CustomerEntity
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [EmailAddress, MaxLength(150)]
    public string? Email { get; set; }

    [Phone, MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}
