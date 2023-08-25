using EventOganizer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventOganizer.JWT;
using EventOganizer.Interfaces;
using EventOganizer.Repositories;

namespace EventOganizer.Controllers
{

    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        public AdminController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        [HttpPost]
        [Route("/CreateTickets")]
        public async Task<IActionResult> CreateTickets([FromBody] DTOs.TicketDTO dto)
        {
            var jwt = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(jwt) || !JwtUtility.ValidateToken(jwt, out var principal))
            {
                return Unauthorized();
            }

            var userRoles = JwtUtility.GetUserRoles(principal);
            if (userRoles.Contains("Admin")) {
                await _ticketRepository.PostTicket(dto);

                return Ok("Saved!");
         
            }
            return BadRequest("Roles could not be extracted.");
        }

        [HttpPut]
        [Route("/Delete/{id}")]
        public async Task<IActionResult> DeleteTicket([FromRoute] int id)
        {
            var jwt = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(jwt) || !JwtUtility.ValidateToken(jwt, out var principal))
            {
                return Unauthorized();
            }

            var userRoles = JwtUtility.GetUserRoles(principal);
            if (userRoles.Contains("Admin"))
            {
                await _ticketRepository.DeleteTicketAsync(id);

                return Ok("Deleted!");

            }
            return BadRequest("Roles could not be extracted.");
        }

        [HttpGet]
        [Route("auth")]
        public IActionResult CheckAdminStatus()
        {
            var jwt = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(jwt) || !JwtUtility.ValidateToken(jwt, out var principal))
            {
                return Unauthorized();
            }

            var userRoles = JwtUtility.GetUserRoles(principal);
            if (userRoles.Contains("Admin"))
            {
                return Ok(new { IsAdmin = true });
            }

            return Ok(new { IsAdmin = false });
        }
    }


}
