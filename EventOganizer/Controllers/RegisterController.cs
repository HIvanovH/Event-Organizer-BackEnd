using EventOganizer.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventOganizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RegisterController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegistration([FromBody] DTOs.RegisterDTO dto)
        {
            var newUser = new Entities.User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                LastName = dto.LastName,

            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));
                }

                await _userManager.AddToRoleAsync(newUser, "Customer");
                return Ok("Registered successfully!");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}
