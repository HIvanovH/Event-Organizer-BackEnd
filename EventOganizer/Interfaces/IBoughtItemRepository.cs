using EventOganizer.DTOs;
using EventOganizer.Entities;

namespace EventOganizer.Interfaces
{

    public interface IBoughtItemRepository
    {
       Task CompleteTransactionAsync(List<CartItem> cartItems, string userId);

       Task<List<EventSummary>> GetPurchaseHistoryByUserIdAsync(string userId);
    }
}
