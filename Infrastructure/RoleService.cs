using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Utility;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class RoleService
    {
        private readonly IUnitOfWork _uow;
        private readonly LoggedInUserId _loggedInUserId;
        private readonly ExecuteSp _executeSp;
        public RoleService(IUnitOfWork uow, LoggedInUserId loggedInUserId, ExecuteSp executeSp)
        {
            _uow = uow;
            _loggedInUserId = loggedInUserId;
            _executeSp = executeSp;
        }

        public async Task<PagedResult<RoleResponse>> GetAllRoles()
        {
            var data = await _uow.RoleRepository.GetAll();
            var response = data.Where(r => r.IsActive).Select(x => new RoleResponse { Id = x.Id, RoleType = x.RoleType }).ToList();
            var totalRecord = 0;
            totalRecord = response.Count();

            return new PagedResult<RoleResponse>(response, totalRecord);
        }
        public async Task<RoleDetailResponse> GetRolesById(int id)
        {
            var roleIdParameter = DataProvider.GetIntSqlParameter("RoleId", id);
            var data = await _executeSp.ExecuteStoredProcedureListAsync<RoleDetailResultSet>("GetRoleDetailsById", roleIdParameter);
            if (data != null)
            {
                var result = data.Select(x => new RoleDetailResponse
                {
                    Id = x.Id,
                    RoleType = x.RoleType,
                    CreatedDate = x.CreatedDate.ToString("dd-MM-yyyy"),
                    ModifiedDate = x.ModifiedDate.ToString("dd-MM-yyyy"),
                    CreatedBy = x.CreatedBy,
                    ModifiedBy = x.ModifiedBy
                }).FirstOrDefault();
                return result;
            }
            return null;
        }

        public async Task<string> AddOrUpdateRole(RoleRequest role)
        {
            var result = "An error occurred while adding or updating the role.";
            try
            {
                var loginData = _loggedInUserId.GetUserAndRole();
                var existingRole = await _uow.RoleRepository.GetRoleByNameOrId(role.RoleId, role.RoleType);

                if (existingRole != null)
                {
                    existingRole.RoleType = role.RoleType;
                    existingRole.ModifiedDate = DateTime.UtcNow;
                    existingRole.ModifiedBy = loginData.userId;
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
                        CreatedBy = loginData.userId,
                        ModifiedBy = loginData.userId,
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

        public async Task<string> DeleteRole(int id)
        {
            var result = "Role not found or already inactive.";
            try
            {
                var loginData = _loggedInUserId.GetUserAndRole();
                var role = await _uow.RoleRepository.GetById(id);

                if (role == null || !role.IsActive)
                {
                    return result;
                }

                role.ModifiedDate = DateTime.UtcNow;
                role.ModifiedBy = loginData.userId;
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
