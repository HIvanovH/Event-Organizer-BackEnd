using System.ComponentModel.DataAnnotations;

namespace EventOganizer.Entities
{
    public class Location
    {
        [Key]
        public int id {  get; set; }
        public string Address { get; set; }
    }
}
