namespace Business.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public int CustomerId { get; set; }
}