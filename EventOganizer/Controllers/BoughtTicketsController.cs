
using EventOganizer.Context;
using EventOganizer.DTOs;
using EventOganizer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EventOrganizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoughtTicketsController : ControllerBase
    {
        private readonly AplicationDBContext _dbContext;
        public BoughtTicketsController(AplicationDBContext dbContext)
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

                foreach (var cartItem in cartItems)
                {
                    var boughtItem = new BoughtItem
                    {
                        UserId = user.Id, 
                        TicketId = cartItem.Id,
                        Quantity = cartItem.Quantity
                    };

                    cartItem.IsBought = true;
                    _dbContext.BoughtItems.Add(boughtItem);
                  
                }

                _dbContext.SaveChanges();

                return Ok("Purchase completed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error completing purchase: {ex.Message}");
            }
        }

        [HttpGet("history/{email}")]
        public async Task<IActionResult> GetPurchaseHistory(string email)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.Email.Equals(email));
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var query = from bo in _dbContext.BoughtItems
                            join t in _dbContext.Tickets on bo.TicketId equals t.Id 
                            where bo.UserId == user.Id
                            group new { bo, t } by new { bo.UserId, bo.TicketId, t.Title, t.Description } into g
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
