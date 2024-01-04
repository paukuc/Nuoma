using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
[ApiController]
[Route("users")]
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
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult List()
    {
        //load entities from DB, convert into listing views
        var ents = _context.Users.ToList();
        return Ok(ents);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
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
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] EditUserModel updatedUser)
    {
        if (updatedUser == null || updatedUser.UserID != id)
        {
            return BadRequest("Invalid user data.");
        }
        if (_context.Users.Any(u => u.Username == updatedUser.Username && u.UserID != updatedUser.UserID))
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
        user.Role = updatedUser.Role;
        _context.Users.Update(user);
        _context.SaveChanges();

        return Ok(user);
    }

    [Authorize(Roles = "Admin, RegisteredUser")]
    [HttpGet("owner/{userId}")]
    [ProducesResponseType(typeof(List<Apartment>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOwnedApartmentsByUserId(int userId)
    {
        var user = _context.Users.Find(userId);
        if (user == null)
        {
            return NotFound($"User with ID {userId} not found.");
        }

        var apartments = _context.Apartments
            .Where(a => a.OwnerID == userId)
            .Include(a => a.Floor)
                .ThenInclude(f => f.Building)
            .ToList();

        if (apartments == null || !apartments.Any())
        {
            return NotFound($"No apartments found for user with ID {userId}.");
        }

        return Ok(apartments);
    }

        [Authorize(Roles = "Admin, RegisteredUser")]
    [HttpGet("renter/{userId}")]
    [ProducesResponseType(typeof(List<Apartment>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetRentedApartmentsByUserId(int userId)
    {
        var user = _context.Users.Find(userId);
        if (user == null)
        {
            return NotFound($"User with ID {userId} not found.");
        }

        var apartments = _context.Apartments
            .Where(a => a.RenterID == userId)
            .Include(a => a.Floor)
                .ThenInclude(f => f.Building)
            .ToList();

        if (apartments == null || !apartments.Any())
        {
            return NotFound($"No apartments found for user with ID {userId}.");
        }

        return Ok(apartments);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
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
        foreach (var floor in floorsToCheck.Distinct().Where(f => f != null))
        {
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
        }

        return Ok();
    }



    [Authorize(Roles = "Admin")]
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
