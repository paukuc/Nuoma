using System.ComponentModel.DataAnnotations;
public class EditUserModel
{
    public int UserID { get; set; }
    
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public UserRole Role { get; set; } 
}
