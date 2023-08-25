using EventOganizer.Context;
using EventOganizer.DTOs;
using EventOganizer.Entities;
using EventOganizer.Exceptions;
using EventOganizer.Interfaces;
using EventOganizer.JWT;
using Jose;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventOrganizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoughtTicketsController : ControllerBase
    {
        private readonly IBoughtItemRepository _boughtItemRepository;
        private readonly IUserRepository _userRepository;
        public BoughtTicketsController(IBoughtItemRepository boughtItemRepository, IUserRepository userRepository)
        {
            
            _boughtItemRepository = boughtItemRepository;
            _userRepository = userRepository;
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompletePurchase([FromBody] PurchaseDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.TokenData) || !JwtUtility.ValidateToken(request.TokenData, out var principal))
                {
                    return Unauthorized();
                }

                var userEmail = JwtUtility.GetUserEmail(principal);

                var user = _userRepository.GetUserByEmail(userEmail);

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                List<CartItem> cartItems = request.CartItems;

               await _boughtItemRepository.CompleteTransactionAsync(cartItems, user.Id);



                return Ok("Purchase completed successfully.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); 
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
                var user = _userRepository.GetUserByEmail(userEmail);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }


                List<EventSummary> eventSummaries = await _boughtItemRepository.GetPurchaseHistoryByUserIdAsync(user.Id);

                return Ok(eventSummaries);

            }
           
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving purchase history: {ex.Message}");
            }
        }
    }
    
}
