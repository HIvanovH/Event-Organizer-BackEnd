namespace EventOganizer.DTOs
{
    public class TicketDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } 
        public string Location { get; set; }
        public string Date { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Time { get; set; }

    }
}