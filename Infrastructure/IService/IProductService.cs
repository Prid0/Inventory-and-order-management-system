using Pim.Model.Dtos;
using Pim.Utility;

namespace Pim.Service.IService
{
    public interface IProductService
    {

        Task<PagedResult<ProductResponse>> GetAllProducts(int from, int to);

        Task<ProductDetailResultSet> GetProductById(int id);
        Task<string> AddOrUpdateProduct(int userId, ProductRequest ur);

        Task<string> DeleteProduct(int userId, int id);
    }
}
