using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Utility;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class ProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly ExecuteSp _executeSp;
        private readonly LoggedInUserId _loggedInUserId;
        public ProductService(IUnitOfWork uow, ExecuteSp executeSp, LoggedInUserId loggedInUserId)
        {
            _uow = uow;
            _executeSp = executeSp;
            _loggedInUserId = loggedInUserId;
        }

        public async Task<PagedResult<ProductResponse>> GetAllProducts(int from, int to)
        {
            var totalRecord = 0;
            var fromParameter = DataProvider.GetIntSqlParameter("From", from);
            var toParameter = DataProvider.GetIntSqlParameter("To", to);
            var totalRecordParameter = DataProvider.GetIntSqlParameter("TotalRecord", totalRecord, true);

            var response = await _executeSp.ExecuteStoredProcedureListAsync<ProductResponse>("GetAllProducts", fromParameter, toParameter, totalRecordParameter);
            if (response != null)
            {
                totalRecord = Convert.ToInt32(totalRecordParameter.Value);
                return new PagedResult<ProductResponse>(response, totalRecord);
            }
            return null;
        }

        public async Task<ProductDetailResponse> GetProductById(int id)
        {
            var productIdParameter = DataProvider.GetIntSqlParameter("ProductId", id);
            var product = await _executeSp.ExecuteStoredProcedureListAsync<ProductDetailResultSet>(
                "GetProductDetails", productIdParameter);
            var response = product.Select(x => new ProductDetailResponse
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Price = x.Price,
                Quantity = x.Quantity,
                Discription = x.Discription ?? "",
                Category = x.Category,
                CreatedDate = x.CreatedDate.ToString("dd-MM-yyyy"),
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate.HasValue ? x.ModifiedDate.Value.ToString("dd-MM-yyyy") : "",
                ModifiedBy = x.ModifiedBy
            }
            ).FirstOrDefault();
            return response;
        }

        public async Task<string> AddOrUpdateProduct(ProductRequest ur)
        {
            var result = "error while adding or updating the user";
            try
            {
                var loginData = _loggedInUserId.GetUserAndRole();
                var isValidCategory = await _uow.CategoryRepository.isValidCategory(ur.CategoryId);
                if (!isValidCategory)
                {
                    result = "Please enter select Category";
                    return result;
                }
                var existingProduct = await _uow.ProductRepository.GetProductByNameOrId(ur.ProductId, ur.Name);

                if (existingProduct != null && existingProduct.IsActive && isValidCategory)
                {
                    existingProduct.Name = ur.Name;
                    existingProduct.CategoryId = ur.CategoryId;
                    existingProduct.Price = ur.Price;
                    existingProduct.Discription = ur.Discription;
                    existingProduct.Quantity = ur.Quantity;
                    existingProduct.ModifiedDate = DateTime.UtcNow;
                    existingProduct.ModifiedBy = loginData.userId;

                    await _uow.ProductRepository.Update(existingProduct);
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
                        CreatedBy = loginData.userId,
                        ModifiedBy = loginData.userId,
                        IsActive = true
                    };
                    await _uow.ProductRepository.Add(product);
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

        public async Task<string> DeleteProduct(int id)
        {
            try
            {
                var loginData = _loggedInUserId.GetUserAndRole();
                var product = await _uow.ProductRepository.GetById(id);

                if (product == null || !product.IsActive)
                {
                    return "product not found or already inactive";
                }

                product.ModifiedDate = DateTime.UtcNow;
                product.ModifiedBy = loginData.userId;
                product.IsActive = false;
                await _uow.ProductRepository.Update(product);
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
