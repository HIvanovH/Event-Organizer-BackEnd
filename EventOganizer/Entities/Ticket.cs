using System;
using System.ComponentModel.DataAnnotations;

namespace EventOganizer.Entities
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}
