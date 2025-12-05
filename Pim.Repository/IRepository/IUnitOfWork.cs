using Pim.Data.Models;

namespace Pim.Repository.IRepository
{
    public interface IUnitOfWork 
    {

        IgenericRepo<Users> Users { get; }
        IgenericRepo<Roles> Roles { get; }
        IgenericRepo<Product> Products { get; }
        IgenericRepo<Order> Orders { get; }
        IgenericRepo<ProductCategory> Categories { get; }
        IgenericRepo<UserRoleMapping> UserRoleMapping { get; }
        IgenericRepo<OrderItem> OrderItem { get; }
        IgenericRepo<ErrorLog> ErrorLog { get; }

        Task SaveAsync();
    }
}


