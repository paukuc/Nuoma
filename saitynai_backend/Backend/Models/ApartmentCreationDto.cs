using System.ComponentModel.DataAnnotations;

public class ApartmentCreationDto
{
    [Required]
    public int BuildingId { get; set; }
    [Required]
    public int FloorNumber { get; set; }
    [Required]
    public int Room { get; set; }
    [Required]
    public int OwnerID { get; set; }
    public int RenterID { get; set; }
}
