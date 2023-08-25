using EventOganizer.DTOs;
using EventOganizer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventOganizer.Interfaces
{
    public interface ITicketRepository
    {
        Task <Ticket> GetByIdAsync(int id);
        Task<List<Ticket>> GetAllTicketsAsync();
        Task PostTicket(TicketDTO dto);

        Task DeleteTicketAsync(int id);
    }
}
