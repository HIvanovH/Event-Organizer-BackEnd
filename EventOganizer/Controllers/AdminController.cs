using EventOganizer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventOganizer.JWT;

namespace EventOganizer.Controllers
{

    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public AdminController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("/CreateTickets")]
        public async Task<IActionResult> CreateTickets([FromBody] DTOs.TicketDTO dto)
        {
            var jwt = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(jwt) || !JwtUtility.ValidateToken(jwt, out var principal))
            {
                return Unauthorized();
            }

            var userRoles = JwtUtility.GetUserRoles(principal);
            if (userRoles.Contains("Admin")) { 
            string combinedDateTimeString = $"{dto.Date} {dto.Time}";

            if (DateTime.TryParseExact(combinedDateTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
            {
                var newTicket = new Entities.Ticket()
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Category = dto.Category,
                    Price = dto.Price,
                    Location = dto.Location,
                    Date = parsedDateTime,
                    Quantity = dto.Quantity,
                };

                await _context.Tickets.AddAsync(newTicket);
                await _context.SaveChangesAsync();

                return Ok("Saved!");
            }
            else
            {
                return BadRequest("Invalid date or time format");
            }
            }
            return BadRequest("Roles could not be extracted.");
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTicketById([FromRoute] long id, [FromBody] DTOs.TicketDTO dto)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(q => q.Id == id);

            if (ticket is null)
            {
                return NotFound("Ticket Not Found!");
            }

            ticket.Title = dto.Title;
            ticket.Description = dto.Description;
            ticket.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok("Ticket Updated Successfully");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTicketById([FromRoute] long id)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(q => q.Id == id);

            if (ticket is null)
            {
                return NotFound("Ticket Not Found!");
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok("Ticket Deleted Successfully");
        }
    }
}
