using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventOganizer.Context;
using EventOganizer.DTOs;
using EventOganizer.Entities;
using EventOganizer.Exceptions;
using EventOganizer.Interfaces;

namespace EventOganizer.Repositories
{
    public class BoughtItemRepository : IBoughtItemRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public BoughtItemRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CompleteTransactionAsync(List<CartItem> cartItems, string userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                foreach (var cartItem in cartItems)
                {
                    var boughtItem = new BoughtItem
                    {
                        UserId = userId,
                        TicketId = cartItem.Id,
                        Quantity = cartItem.Quantity
                    };

                    var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(q => q.Id == cartItem.Id);

                    if (ticket == null)
                    {
                        await transaction.RollbackAsync();
                        throw new NotFoundException($"Ticket with ID {cartItem.Id} not found.");
                    }

                    ticket.Quantity -= cartItem.Quantity;

                    if (ticket.Quantity < 0)
                    {
                        await transaction.RollbackAsync();
                        throw new InvalidOperationException("Not enough tickets available.");
                    }
                    else
                    {
                        cartItem.IsBought = true;
                        _dbContext.BoughtItems.Add(boughtItem);
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<EventSummary>> GetPurchaseHistoryByUserIdAsync(string userId)
        {
            var query = from bo in _dbContext.BoughtItems
                        join t in _dbContext.Tickets on bo.TicketId equals t.Id
                        where bo.UserId.Equals(userId)
                        group new { bo, t } by new { bo.TicketId, t.Title, t.Description } into g
                        select new EventSummary
                        {
                            TicketId = g.Key.TicketId,
                            TotalQuantity = g.Sum(x => x.bo.Quantity),
                            Title = g.Key.Title,
                            Description = g.Key.Description
                        };

            return await query.ToListAsync();
        }
    }
}
