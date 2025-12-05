using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.Category
{
    public interface ICategoryRepository : IRepository<ProductCategory>
    {
        Task<bool> isValidCategory(int id);
        Task<ProductCategory> GetCategoryByNameOrId(int id, string type);
    }
}
