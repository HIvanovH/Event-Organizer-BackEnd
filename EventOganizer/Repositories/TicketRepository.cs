using EventOganizer.Context;
using EventOganizer.DTOs;
using EventOganizer.Entities;
using EventOganizer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EventOganizer.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public TicketRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Ticket> GetByIdAsync(int id)
        {
            return await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _dbContext.Tickets.Where(t=>!t.IsDeleted).ToListAsync();
        }

        public async Task PostTicket(TicketDTO dto)
        {
            string combinedDateTimeString = $"{dto.Date} {dto.Time}";

            if (DateTime.TryParseExact(combinedDateTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
            {
                var newTicket = new Entities.Ticket()
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Category = dto.Category,
                    Price = dto.Price,
                    Location = dto.Location,
                    Date = parsedDateTime,
                    Quantity = dto.Quantity,
                };

                try
                {
                    await _dbContext.Tickets.AddAsync(newTicket);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while saving the new ticket.", ex);
                }
            }
            else
            {
                throw new ArgumentException("Invalid date/time format in DTO.");
            }
        }

        public async Task DeleteTicketAsync(int id)
        {
            var ticketToDelete = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == id);

            if (ticketToDelete != null)
            {
                ticketToDelete.IsDeleted = true;

                await _dbContext.SaveChangesAsync();
            }
        }



    }
}
