using Pim.Model.Dtos;
using Pim.Utility;

namespace Pim.Service.IService
{
    public interface IUserService
    {
        Task<PagedResult<UserResponse>> GetAllUsers(int from, int to);

        Task<UserDetailResultSet> GetUsersById(int id);

        Task<string> AddOrUpdateUser(int userId, UserRequest ur);

        Task<string> DeleteUser(int userId, int id);

        Task<string> ResetPassword(ResetPasswordRequest request);
    }
}