using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Utility;

namespace Pim.Service
{
    public class RoleService
    {
        private readonly IUnitOfWork _uow;
        private readonly LoggedInUserId _loggedInUserId;
        public RoleService(IUnitOfWork uow, LoggedInUserId loggedInUserId)
        {
            _uow = uow;
            _loggedInUserId = loggedInUserId;
        }

        public async Task<Roles> GetRolesById(int id)
        {
            var data = await _uow.RoleRepository.GetById(id);
            if (data != null && data.IsActive)
            {
                return data;
            }
            return null;
        }
        public async Task<IEnumerable<RoleResponse>> GetAllRoles()
        {
            var data = await _uow.RoleRepository.GetAll();
            return data.Where(r => r.IsActive).Select(x => new RoleResponse { Id = x.Id, RoleType = x.RoleType }).ToList();
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
