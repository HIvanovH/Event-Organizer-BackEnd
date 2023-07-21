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

            var newAccount = new Entities.Account()
            {
                Name = dto.Name,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = hashedPassword
            };

            await _context.Accounts.AddAsync(newAccount);
            await _context.SaveChangesAsync();

            return Ok("Saved!");
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Entities.Ticket>> LoginUser([FromRoute] long id, [FromQuery] string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(q => q.Id == id);

            if (account is null)
            {
                return NotFound("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(account);
        }
    }
}
