using EventOganizer.Entities;
using static EventOganizer.Entities.CartItem;

namespace EventOganizer.DTOs
{
    public class PurchaseDTO
    {
        public string Email { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
