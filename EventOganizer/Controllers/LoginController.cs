using EventOganizer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventOganizer.Controllers
{
    public class LoginController : Controller
    {
        private readonly AplicationDBContext _context;

        public LoginController(AplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<Entities.Ticket>> LoginUser([FromBody] DTOs.LoginDTO loginDTO)
        {
            var account = await _context.Users.FirstOrDefaultAsync(q => q.Email == loginDTO.Email);

            if (account is null)
            {
                return NotFound("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, account.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(account);
        }
    }
}
