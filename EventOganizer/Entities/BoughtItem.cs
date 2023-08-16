using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventOganizer.Entities
{
    public class BoughtItem
    {
        [Key]
        public int Id { get; set; }

        public int Quantity { get; set; }
        public int TicketId { get; set; }

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public bool isBought { get; set; }
    }
}
