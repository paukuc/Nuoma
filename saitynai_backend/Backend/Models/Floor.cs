using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Floor
{
    public int FloorID { get; set; }
    public int FloorNumber { get; set; }
    
    public int BuildingID { get; set; }
    public Building Building { get; set; }

[JsonIgnore]
    public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}