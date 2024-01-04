using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Building
{
    public int BuildingID { get; set; }
    [Required]
    public string Address { get; set; }
    public int TotalFloors { get; set; }

    [JsonIgnore]

    public ICollection<Floor> Floors { get; set; } = new List<Floor>();
}