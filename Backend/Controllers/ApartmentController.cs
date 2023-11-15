using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("buildings/{buildingId}/floors/{floorNumber}/apartments")]
public class ApartmentController : ControllerBase
{
    private readonly RentalDbContext _context;
    public ApartmentController(RentalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// List entities.
    /// </summary>
    /// <returns>A list of entities.</returns>
    /// <response code="500">On exception.</response>
    [Authorize(Roles = "Admin, RegisteredUser")]
    [HttpGet]
    [ProducesResponseType(typeof(List<Apartment>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult List(int buildingId, int floorNumber)
    {
        var building = _context.Buildings.Find(buildingId);
        if (building == null)
        {
            return BadRequest($"Building with id {buildingId} doesn't exists");
        }
        var floor = _context.Floors.Where(f => f.BuildingID == buildingId && f.FloorNumber == floorNumber).FirstOrDefault();
        if (floor == null)
        {
            return BadRequest($"Floor with number {floorNumber} in building with id {buildingId} doesn't exists");
        }
        //load entities from DB, convert into listing views
        var ents = _context.Apartments.Where(f => f.FloorID == floor.FloorID).ToList();
        return Ok(ents);
    }

    // [HttpPost]
    // public IActionResult Create(ApartmentCreationDto dto)
    // {
    //     // Check if owner exists
    //     var owner = _context.Users.Find(dto.OwnerID);
    //     if (owner == null)
    //     {
    //         return BadRequest("Invalid owner ID.");
    //     }

    //     // Check if a building with the given address exists
    //     var building = _context.Buildings.FirstOrDefault(b => b.Address == dto.Address);

    //     // If the building doesn't exist, create it
    //     if (building == null)
    //     {
    //         building = new Building { Address = dto.Address };
    //         _context.Buildings.Add(building);
    //         _context.SaveChanges(); // Save to get the generated ID for the building
    //     }

    //     // Check if a floor with the given floor number in that building exists
    //     var floor = _context.Floors.FirstOrDefault(f => f.FloorNumber == dto.FloorNumber && f.BuildingID == building.BuildingID);

    //     // If the floor doesn't exist, create it
    //     if (floor == null)
    //     {
    //         floor = new Floor { FloorNumber = dto.FloorNumber, BuildingID = building.BuildingID };
    //         _context.Floors.Add(floor);
    //         _context.SaveChanges(); // Save to get the generated ID for the floor
    //     }

    //     // Check if an apartment with the same room number exists on this floor
    //     var existingApartment = _context.Apartments.FirstOrDefault(a => a.FloorID == floor.FloorID && a.Room == dto.Room);
    //     if (existingApartment != null)
    //     {
    //         return BadRequest("Apartment with the same room number already exists on this floor.");
    //     }

    //     // Create the apartment and link it to the floor and owner
    //     var apartment = new Apartment
    //     {
    //         FloorID = floor.FloorID,
    //         Room = dto.Room,
    //         OwnerID = owner.UserID
    //     };
    //     _context.Apartments.Add(apartment);
    //     _context.SaveChanges();

    //     return Ok(apartment); // Return the created apartment or some other appropriate response
    // }

    [Authorize(Roles = "Admin")]
    [HttpPut("{apartmentNumber}")]
    [ProducesResponseType(typeof(ApartmentCreationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int apartmentNumber, [FromBody] ApartmentCreationDto dto, int buildingId, int floorNumber)
    {
        if (dto == null)
        {
            return BadRequest("Invalid apartment data.");
        }
        var building = _context.Buildings.Find(buildingId);
        if (building == null)
        {
            return BadRequest($"Building with ID {buildingId} not found.");
        }

        var floor = _context.Floors.Where(f => f.BuildingID == buildingId && f.FloorNumber == floorNumber).FirstOrDefault();
        if (floor == null)
        {
            return BadRequest($"Floor with number {floorNumber} in building {buildingId} not found.");
        }

        var apartment = _context.Apartments.Where(f => f.FloorID == floor.FloorID && f.Room == apartmentNumber).FirstOrDefault();
        if (apartment == null)
        {
            return BadRequest($"An apartment with room number {apartmentNumber} doesn't exists in building {buildingId} floor number {floorNumber}");
        }

        // Check if owner exists
        var owner = _context.Users.Find(dto.OwnerID);
        if (owner == null)
        {
            return BadRequest("Invalid owner ID.");
        }

        // Check if a building with the given address exists
        building = _context.Buildings.FirstOrDefault(b => b.BuildingID == dto.BuildingId);
        if (building == null)
        {
            return BadRequest($"Building with id {dto.BuildingId} not found.");
        }
        floor = _context.Floors.Where(f => f.BuildingID == dto.BuildingId && f.FloorNumber == dto.FloorNumber).FirstOrDefault();
        if (floor == null)
        {
            return BadRequest($"Floor with number {floorNumber} in building {buildingId} not found.");
        }

        // Check if there's another apartment with the same room number on this floor, excluding the current one
        var existingApartment = _context.Apartments.FirstOrDefault(a => a.FloorID == floor.FloorID && a.Room == dto.Room && a.Id != apartment.Id);
        if (existingApartment != null)
        {
            if ((apartment.FloorID != floor.FloorID) && (apartment.Room != dto.Room))
            {
                return BadRequest("Another apartment with the same room number already exists on this floor.");
            }

        }

        // Update the apartment with new details
        apartment.FloorID = floor.FloorID;
        apartment.Room = dto.Room;
        apartment.OwnerID = owner.UserID;

        _context.Apartments.Update(apartment);
        _context.SaveChanges();

        return Ok(apartment);
    }

    // [HttpGet("byfloor/{floorId}")]
    // [ProducesResponseType(typeof(IEnumerable<Apartment>), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // public IActionResult GetApartmentsByFloor(int floorId)
    // {
    //     var floor = _context.Floors.Include(f => f.Apartments).FirstOrDefault(f => f.FloorID == floorId);
    //     if (floor == null)
    //     {
    //         return NotFound($"Floor with ID {floorId} not found.");
    //     }

    //     return Ok(floor.Apartments);
    // }

    // [HttpGet("bybuilding/{buildingId}")]
    // [ProducesResponseType(typeof(IEnumerable<Apartment>), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // public IActionResult GetApartmentsByBuilding(int buildingId)
    // {
    //     var building = _context.Buildings.Include(b => b.Floors).ThenInclude(f => f.Apartments).FirstOrDefault(b => b.BuildingID == buildingId);
    //     if (building == null)
    //     {
    //         return NotFound($"Building with ID {buildingId} not found.");
    //     }

    //     var apartments = building.Floors.SelectMany(f => f.Apartments).ToList();
    //     return Ok(apartments);
    // }

    [Authorize(Roles = "Admin, RegisteredUser")]
    [HttpDelete("{apartmentNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int apartmentNumber, int buildingId, int floorNumber)
    {
        var building = _context.Buildings.Find(buildingId);
        if (building == null)
        {
            return BadRequest($"Building with ID {buildingId} not found.");
        }

        var floor = _context.Floors.Where(f => f.BuildingID == buildingId && f.FloorNumber == floorNumber).FirstOrDefault();
        if (floor == null)
        {
            return BadRequest($"Floor with number {floorNumber} in building {buildingId} not found.");
        }

        var apartmentExists = _context.Apartments.Where(f => f.FloorID == floor.FloorID && f.Room == apartmentNumber).FirstOrDefault();
        if (apartmentExists == null)
        {
            return BadRequest($"An apartment with room number {apartmentNumber} doesn't exists in building {buildingId} floor number {floorNumber}");
        }
        _context.Apartments.Remove(apartmentExists);
        _context.SaveChanges();
        return Ok();
    }
    [Authorize(Roles = "Admin, RegisteredUser")]
    [HttpPost("{ownerId}/{roomNumber}")]
    public IActionResult CreateApartment(int buildingId, int floorNumber, int ownerId, int roomNumber)
    {
        // Check if the specified user (owner) exists.
        var user = _context.Users.Find(ownerId);
        if (user == null)
        {
            return BadRequest($"User with ID {ownerId} not found.");
        }

        var building = _context.Buildings.Find(buildingId);
        if (building == null)
        {
            return BadRequest($"Building with ID {buildingId} not found.");
        }

        // Check if a floor exists with the specified ID.
        var floor = _context.Floors.Where(f => f.BuildingID == buildingId && f.FloorNumber == floorNumber).FirstOrDefault();
        if (floor == null)
        {
            return BadRequest($"Floor with number {floorNumber} in building {buildingId} not found.");
        }

        // Check if an apartment with the same room number already exists on the specified floor.
        var apartmentExists = _context.Apartments.Any(a => a.FloorID == floor.FloorID && a.Room == roomNumber);
        if (apartmentExists)
        {
            return BadRequest($"An apartment with room number {roomNumber} already exists on floor {floorNumber}.");
        }

        // If all checks pass, create the apartment and add it to the context.
        var newApartment = new Apartment
        {
            FloorID = floor.FloorID,
            Room = roomNumber,
            OwnerID = ownerId
            // ... assign other properties as needed ...
        };

        _context.Apartments.Add(newApartment);
        _context.SaveChanges();

        return Ok(newApartment);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{apartmentNumber}")]
    [ProducesResponseType(typeof(Apartment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(int apartmentNumber, int buildingId, int floorNumber)
    {
        var building = _context.Buildings.Find(buildingId);
        if (building == null)
        {
            return BadRequest($"Building with ID {buildingId} not found.");
        }

        var floor = _context.Floors.Where(f => f.BuildingID == buildingId && f.FloorNumber == floorNumber).FirstOrDefault();
        if (floor == null)
        {
            return BadRequest($"Floor with number {floorNumber} in building {buildingId} not found.");
        }

        var apartmentExists = _context.Apartments.Where(f => f.FloorID == floor.FloorID && f.Room == apartmentNumber).FirstOrDefault();
        if (apartmentExists == null)
        {
            return BadRequest($"An apartment with room number {apartmentNumber} doesn't exists in building {buildingId} floor number {floorNumber}");
        }

        return Ok(apartmentExists);
    }


}