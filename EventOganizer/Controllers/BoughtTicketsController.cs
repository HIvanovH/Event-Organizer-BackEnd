using EventOganizer.Context;
using EventOganizer.DTOs;
using EventOganizer.Entities;
using EventOganizer.JWT;
using Microsoft.AspNetCore.Mvc;

namespace EventOrganizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoughtTicketsController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        public BoughtTicketsController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("complete")]
        public IActionResult CompletePurchase([FromBody] PurchaseDTO request)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == request.Email);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var cartItems = request.CartItems;

                using var transaction = _dbContext.Database.BeginTransaction();

                foreach (var cartItem in cartItems)
                {
                    var boughtItem = new BoughtItem
                    {
                        UserId = user.Id,
                        TicketId = cartItem.Id,
                        Quantity = cartItem.Quantity
                    };

                    var ticket = _dbContext.Tickets.FirstOrDefault(q => q.Id == cartItem.Id);

                    if (ticket == null)
                    {
                        transaction.Rollback(); 
                        return NotFound($"Ticket with ID {cartItem.Id} not found.");
                    }

                    ticket.Quantity -= cartItem.Quantity;

                    if (ticket.Quantity < 0)
                    {
                        transaction.Rollback(); 
                        return BadRequest("Not enough tickets available.");
                    }
                    else
                    {
                        cartItem.IsBought = true;
                        _dbContext.BoughtItems.Add(boughtItem);
                    }
                }

                _dbContext.SaveChanges();
                transaction.Commit();

                return Ok("Purchase completed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error completing purchase: {ex.Message}");
            }
        }


        [HttpGet("history")]
        public async Task<IActionResult> GetPurchaseHistory()
        {

            var jwt = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(jwt) || !JwtUtility.ValidateToken(jwt, out var principal))
            {
                return Unauthorized();
            }

            var userEmail = JwtUtility.GetUserEmail(principal);
            try
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.Email.Equals(userEmail));
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var query = from bo in _dbContext.BoughtItems
                            join t in _dbContext.Tickets on bo.TicketId equals t.Id 
                            where bo.UserId == user.Id
                            group new { bo, t } by new { bo.UserId, bo.TicketId, t.Title, t.Description, t.Date, t.Location } into g
                            select new EventSummary 
                            {
                               
                                TicketId = g.Key.TicketId,
                                TotalQuantity = g.Sum(x => x.bo.Quantity),
                                Title = g.Key.Title,
                                Description = g.Key.Description
                            };

                List<EventSummary> eventSummaries = query.ToList();

                return Ok(eventSummaries);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving purchase history: {ex.Message}");
            }
        }
    }
    public class EventSummary
    {
        public int TicketId { get; set; }
        public int TotalQuantity { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
    }
}
