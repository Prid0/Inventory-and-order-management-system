using Microsoft.EntityFrameworkCore;
using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.Role
{
    public class RoleRepository : Repository<Roles>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Roles> GetRoleByNameOrId(int id, string type)
        {
            return await _context.Roles.FirstOrDefaultAsync(c => c.Id == id ||
                                (c.RoleType != null && type != null && c.RoleType.ToLower() == type.ToLower()));

        }
    }
}
