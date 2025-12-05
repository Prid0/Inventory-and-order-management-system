using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Utility;

namespace Pim.Service
{
    public class ProductCategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly LoggedInUserId _loggedInUserId;
        public ProductCategoryService(IUnitOfWork uow, LoggedInUserId loggedInUserId)
        {
            _uow = uow;
            _loggedInUserId = loggedInUserId;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategory()
        {
            var data = await _uow.CategoryRepository.GetAll();
            var categories = data.Where(u => u.IsActive).ToList();
            if (categories != null)
            {
                return categories;
            }
            return null;
        }

        public async Task<ProductCategory> GetCategoryById(int id)
        {
            var categories = await _uow.CategoryRepository.GetById(id);
            if (categories != null && categories.IsActive)
            {
                return categories;
            }
            return null;
        }

        public async Task<string> AddOrUpdateCategory(CategoryRequest ur)
        {
            var result = "error while adding or updating the category";
            try
            {
                var loginData = _loggedInUserId.GetUserAndRole();
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
                    ExistingCategory.ModifiedBy = loginData.userId;
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
                        CreatedBy = loginData.userId,
                        ModifiedBy = loginData.userId,
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

        public async Task<string> DeleteCategory(int id)
        {
            try
            {
                var loginData = _loggedInUserId.GetUserAndRole();
                var category = await _uow.CategoryRepository.GetById(id);

                if (category == null || !category.IsActive)
                {
                    return "Category not found or already inactive";
                }

                category.ModifiedDate = DateTime.UtcNow;
                category.ModifiedBy = loginData.userId;
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
