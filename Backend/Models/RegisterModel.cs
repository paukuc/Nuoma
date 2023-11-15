using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    // Optional: You can allow the user to specify a role during registration or set a default role
    public UserRole Role { get; set; } = UserRole.RegisteredUser; // Default role
}