using Microsoft.EntityFrameworkCore;
using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.Products
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByNameOrId(int id, string name)
        {
            return await _context.Product.FirstOrDefaultAsync(c => c.Id == id ||
                                (c.Name != null && name != null && c.Name.ToLower() == name.ToLower()));

        }
    }
}
