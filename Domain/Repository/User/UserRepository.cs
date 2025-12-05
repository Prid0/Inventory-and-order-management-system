using Microsoft.EntityFrameworkCore;
using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.User
{
    public class UserRepository : Repository<Users>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext Context) : base(Context)
        {
            _context = Context;
        }

        public async Task<UserRoleMapping> GetRoleMappingById(int userId)
        {
            return await _context.UserRoleMapping.FindAsync(userId);
        }

        public async Task AddRoleMapping(UserRoleMapping request)
        {
            await _context.UserRoleMapping.AddAsync(request);
        }

        public async Task UpdateRoleMapping(UserRoleMapping request)
        {
            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Users> GetUserByEmail(string request)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == request);

        }
        public async Task<Users> GetUserByEmailAndPhone(string email, long phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.PhoneNumber == phoneNumber);
        }

        public async Task<bool> CanCreateRole(int creatorRoleId, int newRoleId)
        {
            if (creatorRoleId == 1)
                return true;

            if (creatorRoleId == 2 && newRoleId == 3)
                return true;

            if (creatorRoleId == 0 && newRoleId == 3)
                return true;

            return false;
        }


    }
}
