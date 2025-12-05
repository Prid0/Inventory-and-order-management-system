using Microsoft.EntityFrameworkCore;
using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.Category
{
    public class CategoryRepository : Repository<ProductCategory>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> isValidCategory(int id)
        {
            return await _context.ProductCategory.AnyAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<ProductCategory> GetCategoryByNameOrId(int id, string type)
        {
            return await _context.ProductCategory.FirstOrDefaultAsync(c => c.Id == id ||
                                (c.Type != null && type != null && c.Type.ToLower() == type.ToLower()));

        }
    }
}
