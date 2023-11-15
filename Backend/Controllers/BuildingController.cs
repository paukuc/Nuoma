using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("buildings")]
public class BuildingController : ControllerBase
{
    private readonly RentalDbContext _context;
    public BuildingController(RentalDbContext context)
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
    [ProducesResponseType(typeof(List<Building>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult List()
    {
        //load entities from DB, convert into listing views
        var ents = _context.Buildings.ToList();
        return Ok(ents);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create(CreateBuilding adress)
    {
        // Check if a building with the given address exists
        var building = _context.Buildings.FirstOrDefault(b => b.Address == adress.Address);

        // If the building doesn't exist, create it
        if (building == null)
        {
            building = new Building { Address = adress.Address };
            _context.Buildings.Add(building);
            _context.SaveChanges(); // Save to get the generated ID for the building
        }
        else
        {
            return BadRequest("building with the same adress already exists.");
        }
        return Ok(building); // Return the created apartment or some other appropriate response
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Building), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] CreateBuilding updateBuilding)
    {
        var findBuilding = _context.Buildings.Find(id);
        if (findBuilding == null)
        {
            return NotFound($"Building with ID {id} not found.");
        }

        // Check if another building (excluding the current one) has the same address
        var buildingWithSameAddress = _context.Buildings.FirstOrDefault(b => b.Address == updateBuilding.Address && b.BuildingID != id);

        if (buildingWithSameAddress != null)
        {
            return BadRequest("Building with the same address already exists.");
        }

        findBuilding.Address = updateBuilding.Address;
        _context.SaveChanges();  // Save changes to database

        return Ok(findBuilding);  // Respond with the updated building
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var building = _context.Buildings
    .Include(u => u.Floors)
    .ThenInclude(a => a.Apartments)
    .FirstOrDefault(u => u.BuildingID == id);
        if (building == null)
        {
            return NotFound($"Building with ID {id} not found.");
        }
        var floorsToCheck = new List<Floor>();

        foreach (var floor in building.Floors)
        {
            if (floor.Apartments != null && floor.Apartments.Any())
            {
                _context.Apartments.RemoveRange(floor.Apartments);
            }
        }

        // Remove floors associated with the building
        _context.Floors.RemoveRange(building.Floors);

        // Remove the building itself
        _context.Buildings.Remove(building);
        _context.SaveChanges();

        return Ok();
    }

    [Authorize(Roles = "Admin, RegisteredUser")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Building), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(int id)
    {
        var building = _context.Buildings.Find(id);
        if (building == null)
        {
            return NotFound($"Building with ID {id} not found.");
        }

        return Ok(building);
    }


}