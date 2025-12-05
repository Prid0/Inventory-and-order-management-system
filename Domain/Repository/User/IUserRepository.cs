using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.User
{
    public interface IUserRepository : IRepository<Users>
    {
        Task<UserRoleMapping> GetRoleMappingById(int userId);
        Task AddRoleMapping(UserRoleMapping request);
        Task UpdateRoleMapping(UserRoleMapping request);
        Task<Users> GetUserByEmail(string request);
        Task<Users> GetUserByEmailAndPhone(string email, long phoneNumber);
        Task<bool> CanCreateRole(int creatorRoleId, int newRoleId);

    }
}

