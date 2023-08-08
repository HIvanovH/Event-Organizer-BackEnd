using EventOganizer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventOganizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly AplicationDBContext _context;

        public RegisterController(AplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegistration([FromBody] DTOs.RegisterDTO dto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newAccount = new Entities.User()
            {
                Name = dto.Name,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = hashedPassword
            };

            await _context.Users.AddAsync(newAccount);
            await _context.SaveChangesAsync();

            return Ok("Saved!");
        }

    }
}
