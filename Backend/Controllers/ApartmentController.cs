using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("[controller]")]
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
    [HttpGet("list")]
    [ProducesResponseType(typeof(List<Apartment>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult List()
    {
        //load entities from DB, convert into listing views
        var ents = _context.Apartments.ToList();
        return Ok(ents);
    }

    [HttpPost("create")]
    [HttpPost]
    public IActionResult Create(ApartmentCreationDto dto)
    {
        // Check if owner exists
        var owner = _context.Users.Find(dto.OwnerID);
        if (owner == null)
        {
            return BadRequest("Invalid owner ID.");
        }

        // Check if a building with the given address exists
        var building = _context.Buildings.FirstOrDefault(b => b.Address == dto.Address);

        // If the building doesn't exist, create it
        if (building == null)
        {
            building = new Building { Address = dto.Address };
            _context.Buildings.Add(building);
            _context.SaveChanges(); // Save to get the generated ID for the building
        }

        // Check if a floor with the given floor number in that building exists
        var floor = _context.Floors.FirstOrDefault(f => f.FloorNumber == dto.FloorNumber && f.BuildingID == building.BuildingID);

        // If the floor doesn't exist, create it
        if (floor == null)
        {
            floor = new Floor { FloorNumber = dto.FloorNumber, BuildingID = building.BuildingID };
            _context.Floors.Add(floor);
            _context.SaveChanges(); // Save to get the generated ID for the floor
        }

        // Check if an apartment with the same room number exists on this floor
        var existingApartment = _context.Apartments.FirstOrDefault(a => a.FloorID == floor.FloorID && a.Room == dto.Room);
        if (existingApartment != null)
        {
            return BadRequest("Apartment with the same room number already exists on this floor.");
        }

        // Create the apartment and link it to the floor and owner
        var apartment = new Apartment
        {
            FloorID = floor.FloorID,
            Room = dto.Room,
            OwnerID = owner.UserID
        };
        _context.Apartments.Add(apartment);
        _context.SaveChanges();

        return Ok(apartment); // Return the created apartment or some other appropriate response
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(typeof(ApartmentCreationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] ApartmentCreationDto dto)
    {
        if (dto == null || id <= 0)
        {
            return BadRequest("Invalid apartment data.");
        }

        var apartment = _context.Apartments.Find(id);
        if (apartment == null)
        {
            return NotFound($"Apartment with ID {id} not found.");
        }

        // Check if owner exists
        var owner = _context.Users.Find(dto.OwnerID);
        if (owner == null)
        {
            return BadRequest("Invalid owner ID.");
        }

        // Check if a building with the given address exists
        var building = _context.Buildings.FirstOrDefault(b => b.Address == dto.Address);

        // If the building doesn't exist, create it
        if (building == null)
        {
            building = new Building { Address = dto.Address };
            _context.Buildings.Add(building);
            _context.SaveChanges();
        }

        // Check if a floor with the given floor number in that building exists
        var floor = _context.Floors.FirstOrDefault(f => f.FloorNumber == dto.FloorNumber && f.BuildingID == building.BuildingID);

        // If the floor doesn't exist, create it
        if (floor == null)
        {
            floor = new Floor { FloorNumber = dto.FloorNumber, BuildingID = building.BuildingID };
            _context.Floors.Add(floor);
            _context.SaveChanges();
        }

        // Check if there's another apartment with the same room number on this floor, excluding the current one
        var existingApartment = _context.Apartments.FirstOrDefault(a => a.FloorID == floor.FloorID && a.Room == dto.Room && a.Id != id);
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

    [HttpGet("byfloor/{floorId}")]
    [ProducesResponseType(typeof(IEnumerable<Apartment>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetApartmentsByFloor(int floorId)
    {
        var floor = _context.Floors.Include(f => f.Apartments).FirstOrDefault(f => f.FloorID == floorId);
        if (floor == null)
        {
            return NotFound($"Floor with ID {floorId} not found.");
        }

        return Ok(floor.Apartments);
    }

    [HttpGet("bybuilding/{buildingId}")]
    [ProducesResponseType(typeof(IEnumerable<Apartment>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetApartmentsByBuilding(int buildingId)
    {
        var building = _context.Buildings.Include(b => b.Floors).ThenInclude(f => f.Apartments).FirstOrDefault(b => b.BuildingID == buildingId);
        if (building == null)
        {
            return NotFound($"Building with ID {buildingId} not found.");
        }

        var apartments = building.Floors.SelectMany(f => f.Apartments).ToList();
        return Ok(apartments);
    }


    [HttpDelete("delete/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var apartment = _context.Apartments
    .Include(a => a.Floor)
    .ThenInclude(f => f.Building)
    .FirstOrDefault(a => a.Id == id);
        if (apartment == null)
        {
            return NotFound($"Apartment with ID {id} not found.");
        }
        // Removing associated apartments
        var floor = apartment.Floor;
        _context.Apartments.Remove(apartment);
        _context.SaveChanges();

        _context.Entry(floor).Reload();
        // checks if theres apartments left in the floor and removes it
        if (!floor.Apartments.Any())
        {
            var building = floor.Building;
            _context.Floors.Remove(floor);
            _context.SaveChanges();
            _context.Entry(building).Reload();
            // checks if theres buildings left on the floor and removes it
            if (!building.Floors.Any())
            {
                _context.Buildings.Remove(building);
                _context.SaveChanges();
            }
        }

        return Ok();
    }


    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Apartment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(int id)
    {
        var apartment = _context.Apartments.Find(id);
        if (apartment == null)
        {
            return NotFound($"Apartment with ID {id} not found.");
        }

        return Ok(apartment);
    }


}