using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Service.IService;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _uow;
        private readonly ExecuteSp _executeSp;
        public RoleService(IUnitOfWork uow, ExecuteSp executeSp)
        {
            _uow = uow;
            _executeSp = executeSp;
        }

        public async Task<List<RoleResponse>> GetAllRoles()
        {
            var response = await _executeSp.ExecuteStoredProcedureListAsync<RoleResponse>("GetALLRoles");

            return response;
        }

        public async Task<RoleDetailResultSet> GetRolesById(int id)
        {
            var roleIdParameter = DataProvider.GetIntSqlParameter("RoleId", id);
            return await _executeSp.ExecuteStoredProcedureAsync<RoleDetailResultSet>("GetRoleDetailsById", roleIdParameter);

        }

        public async Task<string> AddOrUpdateRole(int userId, RoleRequest role)
        {
            var result = "An error occurred while adding or updating the role.";
            try
            {
                var existingRole = await _uow.RoleRepository.GetRoleByNameOrId(role.RoleId, role.RoleType);

                if (existingRole != null)
                {
                    existingRole.RoleType = role.RoleType;
                    existingRole.ModifiedDate = DateTime.UtcNow;
                    existingRole.ModifiedBy = userId;
                    existingRole.IsActive = true;

                    await _uow.RoleRepository.Update(existingRole);
                }
                else
                {
                    var newRole = new Roles
                    {
                        RoleType = role.RoleType,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        ModifiedBy = userId,
                        IsActive = true
                    };

                    await _uow.RoleRepository.Add(newRole);
                }

                await _uow.Commit();
                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public async Task<string> DeleteRole(int userId, int id)
        {
            var result = "Role not found or already inactive.";
            try
            {
                var role = await _uow.RoleRepository.GetById(id);

                if (role == null || !role.IsActive)
                {
                    return result;
                }

                role.ModifiedDate = DateTime.UtcNow;
                role.ModifiedBy = userId;
                role.IsActive = false;
                await _uow.RoleRepository.Update(role);
                await _uow.Commit();

                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
