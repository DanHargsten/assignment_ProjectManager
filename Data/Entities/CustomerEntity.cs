namespace Data.Entities;

public class CustomerEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }



    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}
