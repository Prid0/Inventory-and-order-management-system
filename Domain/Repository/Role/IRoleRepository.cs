using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.Role
{
    public interface IRoleRepository : IRepository<Roles>
    {
        Task<Roles> GetRoleByNameOrId(int id, string type);
    }
}
