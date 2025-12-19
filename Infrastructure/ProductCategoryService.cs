using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Service.IService;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ExecuteSp _executeSp;
        public ProductCategoryService(IUnitOfWork unitOfWork, ExecuteSp executeSp)
        {
            _unitOfWork = unitOfWork;
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

            if (string.IsNullOrWhiteSpace(ur.Type))
            {
                result = "category name is required";
                return result;
            }

            var ExistingCategory = await _unitOfWork.CategoryRepository.GetCategoryByNameOrId(ur.CategoryId, ur.Type);

            if (ExistingCategory != null)
            {
                ExistingCategory.Type = ur.Type;
                ExistingCategory.ModifiedDate = DateTime.UtcNow;
                ExistingCategory.ModifiedBy = userId;
                ExistingCategory.IsActive = true;
                await _unitOfWork.CategoryRepository.Update(ExistingCategory);

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
                await _unitOfWork.CategoryRepository.Add(category);
            }
            result = "success";
            await _unitOfWork.Commit();

            return result;
        }

        public async Task<string> DeleteCategory(int userId, int id)
        {
            var result = "Category not found or already inactive";
            var category = await _unitOfWork.CategoryRepository.GetById(id);

            if (category == null || !category.IsActive)
            {
                return result;
            }

            category.ModifiedDate = DateTime.UtcNow;
            category.ModifiedBy = userId;
            category.IsActive = false;
            await _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.Commit();

            result = "success";
            return result;
        }

    }
}
