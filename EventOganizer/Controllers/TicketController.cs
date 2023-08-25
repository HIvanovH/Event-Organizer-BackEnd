using EventOganizer.Context;
using EventOganizer.Entities;
using EventOganizer.Interfaces;
using EventOganizer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventOganizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        
        [HttpGet]

        public async Task<ActionResult<List<Ticket>>> GetAllTickets()
        {
            List<Ticket> tickets = await _ticketRepository.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Ticket>> GetTicketById([FromRoute] int id )
        {
            Ticket ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket is null)
            {
                return NotFound("Ticket Not Found!");
            }

            return Ok(ticket);
        }

       
    }
}
