using Pim.Model.Dtos;

namespace Pim.Service.IService
{
    public interface IRoleService
    {
        Task<List<RoleResponse>> GetAllRoles();

        Task<RoleDetailResultSet> GetRolesById(int id);

        Task<string> AddOrUpdateRole(int userId, RoleRequest role);

        Task<string> DeleteRole(int userId, int id);
    }
}
