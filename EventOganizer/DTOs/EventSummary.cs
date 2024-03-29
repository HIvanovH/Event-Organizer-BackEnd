﻿namespace EventOganizer.DTOs
{
    public class EventSummary
    {
        public int TicketId { get; set; }
        public int TotalQuantity { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string ImagePath { get; set; } 
    }
}
