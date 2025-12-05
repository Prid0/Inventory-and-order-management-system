using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetProductByNameOrId(int id, string name);
    }
}
