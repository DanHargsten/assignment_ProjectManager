using Data.Enums;

namespace Business.Models;

public class EmployeeRegistrationForm
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public EmployeeRole Role { get; set; }
}