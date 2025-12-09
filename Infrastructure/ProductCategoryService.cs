using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Utility;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class ProductCategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly LoggedInUserId _loggedInUserId;
        private readonly ExecuteSp _executeSp;
        public ProductCategoryService(IUnitOfWork uow, LoggedInUserId loggedInUserId, ExecuteSp executeSp)
        {
            _uow = uow;
            _loggedInUserId = loggedInUserId;
            _executeSp = executeSp;
        }

        public async Task<PagedResult<CategoryResponse>> GetAllCategory()
        {
            var data = await _uow.CategoryRepository.GetAll();
            var response = data.Where(u => u.IsActive).Select(x => new CategoryResponse { Id = x.Id, Type = x.Type }).ToList();
            var totalRecord = 0;
            if (response != null)
            {
                totalRecord = response.Count();
                return new PagedResult<CategoryResponse>(response, totalRecord);
            }
            return null;
        }

        public async Task<CategoryDetailResponse> GetCategoryById(int id)
        {
            var idParameter = DataProvider.GetIntSqlParameter("Id", id);
            var resultSet = await _executeSp.ExecuteStoredProcedureListAsync<CategoryDetailResultSet>("GetCategoryById", idParameter);
            if (resultSet != null)
            {
                var response = resultSet.Select(x => new CategoryDetailResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedDate = x.CreatedDate.ToString("ddd-MM-yyyy"),
                    ModifiedDate = x.ModifiedDate.ToString("ddd-MM-yyyy"),
                    CreatedBy = x.CreatedBy,
                    ModifiedBy = x.ModifiedBy

                }).FirstOrDefault();
                return response;
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
