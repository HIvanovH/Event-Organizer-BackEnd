using EventOganizer.Entities;
using Microsoft.AspNetCore.Identity;

namespace EventOganizer.Interfaces
{
    public interface IUserRepository
    {
        IdentityUser GetUserByEmail(string email);
    }
}
