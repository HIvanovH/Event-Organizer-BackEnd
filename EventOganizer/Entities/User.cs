using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace EventOganizer.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }

        public ICollection<BoughtItem> Cart { get; set; } = new List<BoughtItem>();
        
    }
}
