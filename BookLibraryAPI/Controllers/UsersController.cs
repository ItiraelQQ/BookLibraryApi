using BookLibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _key;

        public UsersController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _key = config.GetSection("Jwt:Key").Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var user = new User { 
                UserName = userDto.UserName, 
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Role = "User" //роль по умолчанию
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User registered successfully"});
        }
        [HttpPost("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin(UserDto userDto)
        {
            var user = new User
            {
                UserName = userDto.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Role = "Admin" 
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Admin registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login(UserDto userDto)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == userDto.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { message = tokenString });
        }

    }
}
