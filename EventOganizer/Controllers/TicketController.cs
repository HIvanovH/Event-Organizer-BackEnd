using EventOganizer.Context;
using EventOganizer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventOganizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly AplicationDBContext _context;

        public TicketController(AplicationDBContext context)
        {
            _context = context;
        }

        
        [HttpGet]

        public async Task<ActionResult<List<Entities.Ticket>>> GetAllTickets()
        {
            var tickets = await _context.Tickets.ToListAsync();
            return Ok(tickets);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Entities.Ticket>> GetTicketById([FromRoute] long id )
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(q=>q.Id==id);

            if(ticket is null)
            {
                return NotFound("Ticket Not Found!");
            }

            return Ok(ticket);
        }

       
    }
}
