using EventOganizer.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventOganizer.DTOs
{
    public class CartDTO
    {
     
        public int Id { get; set; }

        public int Quantity { get; set; }
        public int TicketId { get; set; }

        public int UserId { get; set; }
        public bool isBought { get; set; }
    }
}
