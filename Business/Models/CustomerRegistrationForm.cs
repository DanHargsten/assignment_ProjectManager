namespace Business.Models;

public class CustomerRegistrationForm
{
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}