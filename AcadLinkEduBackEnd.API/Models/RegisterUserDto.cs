namespace AcadLinkEduBackEnd.API.Models;

public class RegisterUserDto
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
}
