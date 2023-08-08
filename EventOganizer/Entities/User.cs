using System.ComponentModel.DataAnnotations;
namespace EventOganizer.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<CartItem> Cart { get; set; } = new List<CartItem>();
    }
}
