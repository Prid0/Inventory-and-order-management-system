using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Utility;

namespace Pim.Service
{
    public class UserService
    {
        private readonly IUnitOfWork _uow;
        private readonly LoggedInUserId _loggedInUserId;
        public UserService(IUnitOfWork uow, LoggedInUserId loggedInUserId)
        {
            _uow = uow;
            _loggedInUserId = loggedInUserId;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsers()
        {
            var data = await _uow.UserRepository.GetAll();
            var activeUsers = data.Where(u => u.IsActive).ToList();
            if (activeUsers != null)
            {
                var response = activeUsers.Select(x => new UserResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber
                });
                return response;
            }
            return null;
        }

        public async Task<Users> GetUsersById(int id)
        {
            var user = await _uow.UserRepository.GetById(id);
            if (user != null && user.IsActive)
            {
                return user;
            }
            return null;
        }

        public async Task<string> AddOrUpdateUser(UserRequest ur)
        {
            using var transaction = await _uow.BeginTransactionAsync();
            var result = "error while adding or updating the user";

            try
            {
                var loginData = _loggedInUserId.GetUserAndRole();
                var ExistingUser = await _uow.UserRepository.GetById(ur.Id);
                var existingUserRole = await _uow.UserRepository.GetRoleMappingById(ur.Id);
                bool accessToCreate = await _uow.UserRepository.CanCreateRole(loginData.roleId, ur.RoleId);

                if (!accessToCreate)
                {
                    result = "You are not allowed to create or update this role.";
                    return result;
                }

                if (loginData.roleId == ur.RoleId)

                    if (ExistingUser != null && ExistingUser.IsActive && existingUserRole.IsActive)
                    {
                        ExistingUser.Name = ur.Name;
                        ExistingUser.Email = ur.Email;
                        ExistingUser.PhoneNumber = ur.PhoneNumber;
                        ExistingUser.Password = BCrypt.Net.BCrypt.HashPassword(ur.Password);

                        ExistingUser.ModifiedDate = DateTime.UtcNow;
                        ExistingUser.ModifiedBy = loginData.userId;
                        await _uow.UserRepository.Update(ExistingUser);

                        existingUserRole.RoleId = ur.RoleId;
                        existingUserRole.ModifiedDate = DateTime.UtcNow;
                        existingUserRole.ModifiedBy = loginData.userId;

                        await _uow.UserRepository.UpdateRoleMapping(existingUserRole);
                    }
                    else
                    {
                        var user = new Users
                        {
                            Name = ur.Name,
                            Email = ur.Email,
                            PhoneNumber = ur.PhoneNumber,
                            Password = BCrypt.Net.BCrypt.HashPassword(ur.Password),

                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = loginData.userId,
                            ModifiedDate = DateTime.UtcNow,
                            ModifiedBy = loginData.userId,
                            IsActive = true
                        };

                        await _uow.UserRepository.Add(user);
                        await _uow.Commit();

                        var mapping = new UserRoleMapping
                        {
                            UserId = user.Id,
                            RoleId = ur.RoleId,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            CreatedBy = loginData.userId,
                            ModifiedBy = loginData.userId,
                            IsActive = true
                        };

                        await _uow.UserRepository.AddRoleMapping(mapping);
                    }

                result = "success";
                await _uow.Commit();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result = ex.Message;
            }

            return result;
        }


        public async Task<string> DeleteUser(int id)
        {
            using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                var user = await _uow.UserRepository.GetById(id);
                var existingUserRole = await _uow.UserRepository.GetRoleMappingById(user.Id);
                var loginData = _loggedInUserId.GetUserAndRole();
                if ((user == null || !user.IsActive) && !existingUserRole.IsActive)
                {
                    return "User not found or already inactive";
                }

                user.IsActive = false;
                user.ModifiedDate = DateTime.UtcNow;
                user.ModifiedBy = loginData.userId;
                existingUserRole.IsActive = false;
                existingUserRole.ModifiedDate = DateTime.UtcNow;
                existingUserRole.ModifiedBy = loginData.userId;

                await _uow.UserRepository.UpdateRoleMapping(existingUserRole);
                await _uow.UserRepository.Update(user);
                await _uow.Commit();
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ex.Message;
            }
            return "success";
        }

        public async Task<string> ResetPassword(ResetPasswordRequest request)
        {
            var result = "Invalid email or phone number";
            try
            {
                var ExistingUser = await _uow.UserRepository.GetUserByEmailAndPhone(request.Email, request.PhoneNumber);

                if (ExistingUser != null)
                {
                    if (!ExistingUser.IsActive)
                    {
                        result = $"User with email: {request.Email} exists but is inactive";
                        return result;
                    }

                    ExistingUser.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                    ExistingUser.ModifiedDate = DateTime.UtcNow;
                    ExistingUser.ModifiedBy = ExistingUser.Id;

                    await _uow.UserRepository.Update(ExistingUser);
                    await _uow.Commit();

                    result = "Password reset successful";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }


    }
}
