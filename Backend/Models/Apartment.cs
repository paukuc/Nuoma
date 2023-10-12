using System.ComponentModel.DataAnnotations;

public class Apartment
{
    // ... other properties ...

    public int Id { get; set; }
    [Required]
    public int Room { get; set;  }
    [Required]
    public int OwnerID { get; set; }
    public User Owner { get; set; }

    public int? RenterID { get; set; }  // Nullable because an apartment might not be rented at all times.
    public User Renter { get; set; }
    [Required]
    public int FloorID { get; set; }  // Add this line to link to Floor
    public Floor Floor { get; set; }  // Navigation property for Floor
}