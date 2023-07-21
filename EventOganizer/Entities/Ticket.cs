using System;
using System.ComponentModel.DataAnnotations;

namespace EventOganizer.Entities
{
    public class Ticket
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
