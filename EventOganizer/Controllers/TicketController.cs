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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TicketController(AplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        
        [HttpGet]

        public async Task<ActionResult<List<Entities.Ticket>>> GetAllTickets()
        {
            var ticket = await _context.Tickets.ToListAsync();
            return Ok(ticket);
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
