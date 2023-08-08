using EventOganizer.Context;
using EventOganizer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventOganizer.Controllers
{
    [ApiController]
    [Route("api/boughtItems")]
    public class BoughtTicketsController : Controller
    {
        private readonly AplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BoughtTicketsController(AplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<CartItem[]>> GetBoughtItems()
        {
            int.TryParse(_httpContextAccessor.HttpContext.Request.Cookies["userId"], out int userID);
            var cartItems = await _context.CartItems.
            Where(cartItem => cartItem.UserId == userID && cartItem.isBought)
            .Join(
             _context.Tickets,
            cartItem => cartItem.TicketId,
            ticket => ticket.Id,
            (cartItem, ticket) => ticket)
            .ToListAsync();


            return Ok(cartItems);
        }
    }
}
