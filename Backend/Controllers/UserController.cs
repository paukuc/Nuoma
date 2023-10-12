using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly RentalDbContext _context;
    public UserController(RentalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// List entities.
    /// </summary>
    /// <returns>A list of entities.</returns>
    /// <response code="500">On exception.</response>
    [HttpGet("list")]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult List()
    {
        //load entities from DB, convert into listing views
        var ents = _context.Users.ToList();
        return Ok(ents);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest("Invalid user data.");
        }
        if (_context.Users.Any(u => u.Username == user.Username))
        {
            return BadRequest("Username already exists");
        }

        _context.Users.Add(user);
        _context.SaveChanges();

        return CreatedAtAction(nameof(List), new { id = user.UserID }, user);
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] User updatedUser)
    {
        if (updatedUser == null || updatedUser.UserID != id)
        {
            return BadRequest("Invalid user data.");
        }
        if (_context.Users.Any(u => u.Username == updatedUser.Username))
        {
            return BadRequest("Username already exists");
        }

        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        // Update the user's fields here, for example:
        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;

        _context.Users.Update(user);
        _context.SaveChanges();

        return Ok(user);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var user = _context.Users
    .Include(u => u.OwnedApartments)
    .ThenInclude(a => a.Floor)
    .ThenInclude(f => f.Building)
    .Include(u => u.RentedApartments)
    .FirstOrDefault(u => u.UserID == id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }
        var floorsToCheck = new List<Floor>();

        // Removing associated apartments
        if (user.OwnedApartments != null && user.OwnedApartments.Any())
        {
            floorsToCheck.AddRange(user.OwnedApartments.Select(ap => ap.Floor));
            _context.Apartments.RemoveRange(user.OwnedApartments);
        }

        _context.Users.Remove(user);
        _context.SaveChanges();
        foreach (var floor in floorsToCheck.Distinct().Where(f => f!=null))
        {
            _context.Entry(floor).Reload();
            // checks if theres apartments left in the floor and removes it
            if(!floor.Apartments.Any())
            {
                var building = floor.Building;
                _context.Floors.Remove(floor);
                _context.SaveChanges();
                _context.Entry(building).Reload();
                // checks if theres buildings left on the floor and removes it
                if(!building.Floors.Any())
                {
                    _context.Buildings.Remove(building);
                    _context.SaveChanges();
                }
            }
        }

        return Ok();
    }




    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        return Ok(user);
    }


}
