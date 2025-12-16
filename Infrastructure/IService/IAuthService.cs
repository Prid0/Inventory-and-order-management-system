using Pim.Model.Dtos;

namespace Pim.Service.IService
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(string email, string password);

    }
}
