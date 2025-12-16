using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Service.IService;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ExecuteSp _executeSp;
        public RoleService(IUnitOfWork unitOfWork, ExecuteSp executeSp)
        {
            _unitOfWork = unitOfWork;
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
                var existingRole = await _unitOfWork.RoleRepository.GetRoleByNameOrId(role.RoleId, role.RoleType);

                if (existingRole != null)
                {
                    existingRole.RoleType = role.RoleType;
                    existingRole.ModifiedDate = DateTime.UtcNow;
                    existingRole.ModifiedBy = userId;
                    existingRole.IsActive = true;

                    await _unitOfWork.RoleRepository.Update(existingRole);
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

                    await _unitOfWork.RoleRepository.Add(newRole);
                }

                await _unitOfWork.Commit();
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
                var role = await _unitOfWork.RoleRepository.GetById(id);

                if (role == null || !role.IsActive)
                {
                    return result;
                }

                role.ModifiedDate = DateTime.UtcNow;
                role.ModifiedBy = userId;
                role.IsActive = false;
                await _unitOfWork.RoleRepository.Update(role);
                await _unitOfWork.Commit();

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
