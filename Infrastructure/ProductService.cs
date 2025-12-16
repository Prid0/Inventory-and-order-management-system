using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Service.IService;
using Pim.Utility;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ExecuteSp _executeSp;
        private readonly CacheService _cacheService;

        public ProductService(IUnitOfWork unitOfWork, ExecuteSp executeSp, CacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _executeSp = executeSp;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<ProductResponse>> GetAllProducts(int from, int to)
        {
            string cacheKey = $"product_{from}_{to}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var totalRecord = 0;
                    var fromParameter = DataProvider.GetIntSqlParameter("From", from);
                    var toParameter = DataProvider.GetIntSqlParameter("To", to);
                    var totalRecordParameter = DataProvider.GetIntSqlParameter("TotalRecord", totalRecord, true);

                    var response = await _executeSp.ExecuteStoredProcedureListAsync<ProductResponse>("GetAllProducts", fromParameter, toParameter, totalRecordParameter);

                    totalRecord = Convert.ToInt32(totalRecordParameter.Value);
                    return new PagedResult<ProductResponse>(response, totalRecord);

                });
        }

        public async Task<ProductDetailResultSet> GetProductById(int id)
        {
            string cacheKey = $"users_{id}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var productIdParameter = DataProvider.GetIntSqlParameter("ProductId", id);
                    return await _executeSp.ExecuteStoredProcedureAsync<ProductDetailResultSet>(
                        "GetProductDetails", productIdParameter);
                });
        }

        public async Task<string> AddOrUpdateProduct(int userId, ProductRequest ur)
        {
            var result = "error while adding or updating the user";
            try
            {
                var isValidCategory = await _unitOfWork.CategoryRepository.isValidCategory(ur.CategoryId);
                if (!isValidCategory)
                {
                    result = "Please enter select Category";
                    return result;
                }
                var existingProduct = await _unitOfWork.ProductRepository.GetProductByNameOrId(ur.ProductId, ur.Name);

                if (existingProduct != null && existingProduct.IsActive && isValidCategory)
                {
                    existingProduct.Name = ur.Name;
                    existingProduct.CategoryId = ur.CategoryId;
                    existingProduct.Price = ur.Price;
                    existingProduct.Discription = ur.Discription;
                    existingProduct.Quantity = ur.Quantity;
                    existingProduct.ModifiedDate = DateTime.UtcNow;
                    existingProduct.ModifiedBy = userId;

                    await _unitOfWork.ProductRepository.Update(existingProduct);
                }
                else
                {
                    var product = new Product
                    {
                        Name = ur.Name,
                        CategoryId = ur.CategoryId,
                        Price = ur.Price,
                        Discription = ur.Discription,
                        Quantity = ur.Quantity,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        ModifiedBy = userId,
                        IsActive = true
                    };
                    await _unitOfWork.ProductRepository.Add(product);
                }

                await _unitOfWork.Commit();

                _cacheService.Remove($"products_{ur.ProductId}");

                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public async Task<string> DeleteProduct(int userId, int id)
        {
            var result = "error";
            try
            {
                var product = await _unitOfWork.ProductRepository.GetById(id);

                if (product == null || !product.IsActive)
                {
                    return "product not found or already inactive";
                }

                product.ModifiedDate = DateTime.UtcNow;
                product.ModifiedBy = userId;
                product.IsActive = false;
                await _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.Commit();

                _cacheService.Remove($"products_{id}");

                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
