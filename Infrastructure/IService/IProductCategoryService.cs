using Pim.Model.Dtos;

namespace Pim.Service.IService
{
    public interface IProductCategoryService
    {
        Task<List<CategoryResponse>> GetAllCategory();

        Task<CategoryDetailResultSet> GetCategoryById(int id);

        Task<string> AddOrUpdateCategory(int userId, CategoryRequest ur);

        Task<string> DeleteCategory(int userId, int id);
    }
}
