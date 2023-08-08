using EventOganizer.Context;
using EventOganizer.DTOs;
using EventOganizer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace EventOganizer.Controllers
{
    [Route("Cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(AplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<CartDTO[]>> GetCartItems()
        {
            int.TryParse(_httpContextAccessor.HttpContext.Request.Cookies["userId"], out int userID);
            var cartItems = await _context.CartItems
     .Where(cartItem => cartItem.UserId == userID && cartItem.isBought == false)
     .Select(cartItem => new CartItem
     {
         Id = cartItem.Id,
         Quantity = cartItem.Quantity,
         TicketId = cartItem.TicketId,
         UserId = cartItem.UserId,
         isBought = cartItem.isBought
     })
     .ToListAsync();



            return Ok(cartItems);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<CartItem>>> GetItemDetails(int id)
        {
            var item = await _context.CartItems.Where(item=>item.UserId==id).ToListAsync();
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateCartItem(int ticketId, string action)
        {
            try
            {
                int.TryParse(_httpContextAccessor.HttpContext.Request.Cookies["userId"], out int userId);

                var ticket = await _context.Tickets.FindAsync(ticketId);
                if (ticket == null)
                {
                    return NotFound("Ticket not found");
                }

                var cartItem = await _context.CartItems.FirstOrDefaultAsync(item => item.UserId == userId && item.TicketId == ticketId && item.isBought == false);

                if (action.Equals("add", StringComparison.OrdinalIgnoreCase))
                {
                    if (cartItem == null)
                    {
                        cartItem = new CartItem
                        {
                            UserId = userId,
                            TicketId = ticketId,
                            Quantity = 1,
                        };
                        _context.CartItems.Add(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity++;
                    }
                }
                else if (action.Equals("decrease", StringComparison.OrdinalIgnoreCase))
                {
                    if (cartItem == null)
                    {
                        return NotFound("Cart item not found");
                    }

                    if (cartItem.Quantity == 1)
                    {
                        _context.CartItems.Remove(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity--;
                    }
                }
                else if (action.Equals("increase", StringComparison.OrdinalIgnoreCase))
                {
                    if (cartItem == null)
                    {
                        return NotFound("Cart item not found");
                    }
                    cartItem.Quantity++;
                }else if (action.Equals("remove", StringComparison.OrdinalIgnoreCase))
                {
                    if (cartItem == null)
                    {
                        return NotFound("Cart item not found");
                    }

                    if (cartItem.Quantity == 1)
                    {
                        _context.CartItems.Remove(cartItem);
                    }
                    else
                    {
                        _context.CartItems.Remove(cartItem);
                    }

                }
                else
                {
                    return BadRequest("Invalid action");
                }
            

                await _context.SaveChangesAsync();
                return Ok("Cart item updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
