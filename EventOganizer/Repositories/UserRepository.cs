using EventOganizer.Context;
using EventOganizer.Entities;
using EventOganizer.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EventOganizer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public UserRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IdentityUser GetUserByEmail(string email)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Email == email);
        }

    }
}
