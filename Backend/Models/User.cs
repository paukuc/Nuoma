using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public enum UserRole
{
    Admin = 0,
    RegisteredUser = 1
}
public class User
{
    public int UserID { get; set; }
    
    [Required]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }  // Preferably hashed
    [Required]
    public string Email { get; set; }
    [Required]
    public UserRole Role { get; set; }  // e.g. "Registered", "Admin"

    // Relationships
    public ICollection<Apartment> OwnedApartments { get; set; } = new List<Apartment>();
    public ICollection<Apartment> RentedApartments { get; set; } = new List<Apartment>();
}