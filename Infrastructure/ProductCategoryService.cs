using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Service.IService;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly ExecuteSp _executeSp;
        public ProductCategoryService(IUnitOfWork uow, ExecuteSp executeSp)
        {
            _uow = uow;
            _executeSp = executeSp;
        }

        public async Task<List<CategoryResponse>> GetAllCategory()
        {
            var data = await _executeSp.ExecuteStoredProcedureListAsync<CategoryResponse>("GetALLCategories");
            return data;
        }

        public async Task<CategoryDetailResultSet> GetCategoryById(int id)
        {
            var idParameter = DataProvider.GetIntSqlParameter("Id", id);
            var result = await _executeSp.ExecuteStoredProcedureAsync<CategoryDetailResultSet>("GetCategoryById", idParameter);
            return result;
        }

        public async Task<string> AddOrUpdateCategory(int userId, CategoryRequest ur)
        {
            var result = "error while adding or updating the category";
            try
            {
                if (string.IsNullOrWhiteSpace(ur.Type))
                {
                    result = "category name is required";
                    return result;
                }

                var ExistingCategory = await _uow.CategoryRepository.GetCategoryByNameOrId(ur.CategoryId, ur.Type);

                if (ExistingCategory != null)
                {
                    ExistingCategory.Type = ur.Type;
                    ExistingCategory.ModifiedDate = DateTime.UtcNow;
                    ExistingCategory.ModifiedBy = userId;
                    ExistingCategory.IsActive = true;
                    await _uow.CategoryRepository.Update(ExistingCategory);

                    result = "category updated successfully";
                }
                else
                {
                    var category = new ProductCategory
                    {
                        Type = ur.Type,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        ModifiedBy = userId,
                        IsActive = true
                    };
                    await _uow.CategoryRepository.Add(category);
                }
                result = "success";
                await _uow.Commit();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public async Task<string> DeleteCategory(int userId, int id)
        {
            try
            {
                var category = await _uow.CategoryRepository.GetById(id);

                if (category == null || !category.IsActive)
                {
                    return "Category not found or already inactive";
                }

                category.ModifiedDate = DateTime.UtcNow;
                category.ModifiedBy = userId;
                category.IsActive = false;
                await _uow.CategoryRepository.Update(category);
                await _uow.Commit();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }

    }
}
