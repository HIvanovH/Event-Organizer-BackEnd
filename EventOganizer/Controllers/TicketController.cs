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

        [HttpPost]
        public async Task<IActionResult> CreateTickets([FromBody] DTOs.TicketDTO dto)
        {
            //if(dto!=null)
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

            return Ok("Ticket Updated Successfuly");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProductById([FromRoute] long id)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(q => q.Id == id);

            if (ticket is null)
            {
                return NotFound("Ticket Not Found!");
            }

             _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok("Ticket Deleted Successfuly");
        }
    }
}
