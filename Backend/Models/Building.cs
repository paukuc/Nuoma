using System.ComponentModel.DataAnnotations;

public class Building
{
    public int BuildingID { get; set; }
    [Required]
    public string Address { get; set; }
    public int TotalFloors { get; set; }

    public ICollection<Floor> Floors { get; set; } = new List<Floor>();
}