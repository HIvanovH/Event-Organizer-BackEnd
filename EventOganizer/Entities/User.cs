using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace EventOganizer.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }

        
    }
}
