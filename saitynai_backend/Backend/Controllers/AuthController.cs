using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly RentalDbContext _context;

    public AuthController(IConfiguration configuration, RentalDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterModel register)
    {
        // Check if a user with the same username already exists
        if (_context.Users.Any(u => u.Username == register.Username))
        {
            return BadRequest("Username already taken.");
        }

        // Create a new user object
        var user = new User
        {
            Username = register.Username,
            // Other fields can be set here
        };

        // Hash the password
        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, register.Password);
        user.Email = register.Email;
        user.Role = UserRole.RegisteredUser;

        // Save the user in the database
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == login.Username);
        if (user == null)
        {
            return Unauthorized();
        }
        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);

        if (result == PasswordVerificationResult.Success)
        {
            var token = GenerateJWTToken(login.Username, user.Role, user.UserID);
            return Ok(new { token });
        }
        else
        {
            return Unauthorized();
        }
    }

    private string GenerateJWTToken(string username, UserRole role, int userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim("UserId", userId.ToString()) // Add user ID claim
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60), // Set token expiry
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
