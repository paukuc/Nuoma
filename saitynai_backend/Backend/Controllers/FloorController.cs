using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("buildings/{buildingId}/floors")]
public class FloorController : ControllerBase
{
    private readonly RentalDbContext _context;
    public FloorController(RentalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// List entities.
    /// </summary>
    /// <returns>A list of entities.</returns>
    /// <response code="500">On exception.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Floor>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult List(int buildingId)
    {
        //load entities from DB, convert into listing views
        var building = _context.Buildings.Find(buildingId);
        if(building == null)
        {
            return BadRequest($"building with id {buildingId} doesn't exists");
        }
        var floors = _context.Floors.Where(f => f.BuildingID == building.BuildingID);
        return Ok(floors);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{floorNumber}")]
    public IActionResult Create(int floorNumber, int buildingId)
    {
        var building = _context.Buildings.Find(buildingId);
        if(building == null)
        {
            return BadRequest($"building with id {buildingId} doesn't exists");
        }
        Floor creatingFloor = new Floor
        {
            FloorNumber = floorNumber,
            BuildingID = buildingId
        };
        // Check if a floor with the same number exists in the specified building
        var floorExists = _context.Floors.Any(f => f.FloorNumber == creatingFloor.FloorNumber && f.BuildingID == creatingFloor.BuildingID);
        if (floorExists)
        {
            return BadRequest($"A floor with the number {creatingFloor.FloorNumber} already exists in building {creatingFloor.BuildingID}.");
        }

        // Assign the building to the creatingFloor before adding to the context.
        creatingFloor.Building = building;

        _context.Floors.Add(creatingFloor);
        _context.SaveChanges();

        // Nullifying Building before sending a response to reduce payload, if you desire.
        creatingFloor.Building = null;

        return Ok(creatingFloor);
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{floorNumber}")]
    public IActionResult Update(int floorNumber, FloorCreationDto floorUpdateDto, int buildingId)
    {        
        var building = _context.Buildings.Find(buildingId);
        if(building == null)
        {
            return BadRequest($"building with id {buildingId} doesn't exists, floor can't be found");
        }
        // Retrieve the floor from the database.
        var existingFloor = _context.Floors
            .Where(f => f.Building.BuildingID == building.BuildingID && f.FloorNumber == floorNumber).FirstOrDefault();

        if (existingFloor == null)
        {
            return NotFound($"Floor with number {floorNumber} in building with id {buildingId} not found.");
        }
        building = _context.Buildings.Find(floorUpdateDto.BuildingID);
        if(building == null)
        {
            return BadRequest($"building with id {floorUpdateDto.BuildingID} doesn't exists");
        }

        // Check if the updated floor number conflicts with another floor in the same building.
        var floorExists = _context.Floors.Any(f => f.FloorNumber == floorUpdateDto.FloorNumber
                                                 && f.BuildingID == floorUpdateDto.BuildingID
                                                 && f.FloorID != existingFloor.FloorID); // Exclude the current floor
        if (floorExists)
        {
            return BadRequest($"A floor with the number {floorUpdateDto.FloorNumber} already exists in building {floorUpdateDto.BuildingID}.");
        }

        // Update the floor properties with the DTO values.
        existingFloor.FloorNumber = floorUpdateDto.FloorNumber;
        existingFloor.BuildingID = floorUpdateDto.BuildingID;
        existingFloor.Building = building;

        // Save changes to the database.
        _context.SaveChanges();

        return Ok(existingFloor);
    }



    [Authorize(Roles = "Admin")]
    [HttpDelete("{floorNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int floorNumber, int buildingId)
    {
        var building = _context.Buildings.Find(buildingId);
        if(building == null)
        {
            return BadRequest($"building with id {buildingId} doesn't exists, floor can't be found");
        }
        var existingFloor = _context.Floors
            .Where(f => f.Building.BuildingID == building.BuildingID && f.FloorNumber == floorNumber).FirstOrDefault();

        if (existingFloor == null)
        {
            return NotFound($"Floor with number {floorNumber} in building with id {buildingId} not found.");
        }
        _context.Apartments.RemoveRange(existingFloor.Apartments);


        // Remove floors associated with the building
        _context.Floors.Remove(existingFloor);
        _context.SaveChanges();

        return Ok();
    }

    [Authorize(Roles = "Admin, RegisteredUser")]
    [HttpGet("{floorNumber}")]
    [ProducesResponseType(typeof(Floor), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(int floorNumber, int buildingId)
    {
        var building = _context.Buildings.Find(buildingId);
        if(building == null)
        {
            return BadRequest($"building with id {buildingId} doesn't exists, floor can't be found");
        }
        var existingFloor = _context.Floors
            .Where(f => f.Building.BuildingID == building.BuildingID && f.FloorNumber == floorNumber).FirstOrDefault();

        if (existingFloor == null)
        {
            return NotFound($"Floor with number {floorNumber} in building with id {buildingId} not found.");
        }

        return Ok(existingFloor);
    }


}