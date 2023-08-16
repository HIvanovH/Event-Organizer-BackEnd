using EventOganizer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace EventOganizer.Controllers
{
   
    public class AdminController : Controller
    {
        private readonly AplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(AplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("/CreateTickets")]
        public async Task<IActionResult> CreateTickets([FromBody] DTOs.TicketDTO dto)
        {
            DateTime parsedDate = DateTime.Parse(dto.Date);
            int.TryParse(_httpContextAccessor.HttpContext.Request.Cookies["userId"], out int userID);

            var newTicket = new Entities.Ticket()
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                Price = dto.Price,
                Location = dto.Location,
                Date = parsedDate,
            };

            await _context.Tickets.AddAsync(newTicket);
            await _context.SaveChangesAsync();

            return Ok("Saved!");
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
