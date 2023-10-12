using System.ComponentModel.DataAnnotations;

public class ApartmentCreationDto
{
    [Required]
    public string Address { get; set; }
    [Required]
    public int FloorNumber { get; set; }
    [Required]
    public int Room { get; set; }
    [Required]
    public int OwnerID { get; set; }
}
