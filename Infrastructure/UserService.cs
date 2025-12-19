using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Service.IService;
using Pim.Utility;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ExecuteSp _executeSp;
        private readonly CacheService _cacheService;
        public UserService(IUnitOfWork unitOfWork, ExecuteSp executeSp, CacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _executeSp = executeSp;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<UserResponse>> GetAllUsers(int from, int to)
        {
            string cacheKey = $"users_{from}_{to}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var totalRecord = 0;

                    var fromParameter = DataProvider.GetIntSqlParameter("From", from);
                    var toParameter = DataProvider.GetIntSqlParameter("To", to);
                    var totalRecordParameter = DataProvider.GetIntSqlParameter("TotalRecord", totalRecord, true);

                    var result = await _executeSp.ExecuteStoredProcedureListAsync<UserResponse>(
                        "GetAllUsers",
                        fromParameter,
                        toParameter,
                        totalRecordParameter
                    );
                    totalRecord = Convert.ToInt32(totalRecordParameter.Value);
                    return new PagedResult<UserResponse>(result, totalRecord);
                },
                expiration: TimeSpan.FromMinutes(5)
            );
        }

        public async Task<UserDetailResultSet> GetUsersById(int id)
        {
            string cacheKey = $"users_{id}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var idParameter = DataProvider.GetIntSqlParameter("Id", id);

                    return await _executeSp
                        .ExecuteStoredProcedureAsync<UserDetailResultSet>(
                            "GetUserDetail",
                            idParameter);
                },
                expiration: TimeSpan.FromMinutes(5)
            );
        }



        public async Task<string> AddOrUpdateUser(int userId, UserRequest ur)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            var result = "error while adding or updating the user";

            try
            {
                var ExistingUser = await _unitOfWork.UserRepository.GetById(ur.Id);
                var existingUserRole = await _unitOfWork.UserRepository.GetRoleMappingById(ur.Id);

                if (ExistingUser != null && ExistingUser.IsActive && existingUserRole.IsActive)
                {
                    ExistingUser.Name = ur.Name;
                    ExistingUser.Email = ur.Email;
                    ExistingUser.PhoneNumber = ur.PhoneNumber;
                    ExistingUser.Password = BCrypt.Net.BCrypt.HashPassword(ur.Password);

                    ExistingUser.ModifiedDate = DateTime.UtcNow;
                    ExistingUser.ModifiedBy = userId;
                    await _unitOfWork.UserRepository.Update(ExistingUser);

                    existingUserRole.RoleId = ur.RoleId;
                    existingUserRole.ModifiedDate = DateTime.UtcNow;
                    existingUserRole.ModifiedBy = userId;

                    await _unitOfWork.UserRepository.UpdateRoleMapping(existingUserRole);
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
                        CreatedBy = userId,
                        ModifiedDate = DateTime.UtcNow,
                        ModifiedBy = userId,
                        IsActive = true
                    };

                    await _unitOfWork.UserRepository.Add(user);
                    await _unitOfWork.Commit();

                    var mapping = new UserRoleMapping
                    {
                        UserId = user.Id,
                        RoleId = ur.RoleId,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        ModifiedBy = userId,
                        IsActive = true
                    };

                    await _unitOfWork.UserRepository.AddRoleMapping(mapping);
                }

                await _unitOfWork.Commit();
                await transaction.CommitAsync();
                _cacheService.Remove($"users_{ur.Id}");
                result = "success";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result = ex.Message;
            }

            return result;
        }


        public async Task<string> DeleteUser(int userId, int id)
        {
            var result = "User not found or already inactive";
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.UserRepository.GetById(id);
            var existingUserRole = await _unitOfWork.UserRepository.GetRoleMappingById(user.Id);
            if ((user == null || !user.IsActive) && !existingUserRole.IsActive)
            {
                return result;
            }

            user.IsActive = false;
            user.ModifiedDate = DateTime.UtcNow;
            user.ModifiedBy = userId;
            existingUserRole.IsActive = false;
            existingUserRole.ModifiedDate = DateTime.UtcNow;
            existingUserRole.ModifiedBy = userId;

            await _unitOfWork.UserRepository.UpdateRoleMapping(existingUserRole);
            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.Commit();
            await transaction.CommitAsync();
            _cacheService.Remove($"users_{id}");
            result = "success";

            return result;
        }

        public async Task<string> ResetPassword(ResetPasswordRequest request)
        {
            var result = "Invalid email or phone number";

            var ExistingUser = await _unitOfWork.UserRepository.GetUserByEmailAndPhone(request.Email, request.PhoneNumber);

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

                await _unitOfWork.UserRepository.Update(ExistingUser);
                await _unitOfWork.Commit();
                _cacheService.Remove($"users_{ExistingUser.Id}");
                result = "Password reset successful";
            }

            return result;
        }


    }
}
