using Microsoft.EntityFrameworkCore;
using StreamingTitles.Data.Model;

namespace StreamingTitles.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TitlesContext _ctx;

        public CategoryRepository(TitlesContext ctx)
        {
            _ctx = ctx;
        }


        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var categories = await _ctx.Categories.AsNoTracking().ToListAsync();
            return categories;
        }
        public async Task<Category> DropCategoryAsync()
        {
            var category = await _ctx.Categories.FirstOrDefaultAsync();
            _ctx.Categories.Remove(category);
            await _ctx.SaveChangesAsync();
            return category;
        }
        public async Task<Category> GetCategoryById(int categoryid)
        {
            Category? category = await _ctx.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == categoryid);
            return category;

        }

        public async Task<ICollection<Title>> GetTitlesByCategoryId(int categoryid)
        {
            return _ctx.TitleCategories.AsNoTracking().Where(tc => tc.CategoryId == categoryid).Select(tc => tc.Title).ToList();
        }

        public Task<bool> CreateCategory(Category category)
        {
            _ctx.Add(category);
            return Save();
        }

        public Task<bool> Save()
        {
            var saved = _ctx.SaveChangesAsync();
            return saved.ContinueWith(task => task.Result > 0);
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            _ctx.Categories.Update(category);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<Category> GetCategoryByName(string categoryName)
        {
            Category? category = await _ctx.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Name == categoryName);
            return category;
        }

        public Task<bool> DeleteCategory(Category category)
        {
            _ctx.Categories.Remove(category);
            return Save();
        }

        public Task<bool> CategoryExists(int categoryid)
        {
            return _ctx.Categories.AnyAsync(c => c.Id == categoryid);
        }

        public Task<bool> CategoryTitleExists(int testCatId, int testTitleId)
        {
            return _ctx.TitleCategories.AsNoTracking().AnyAsync(tc => tc.CategoryId == testCatId && tc.TitleId == testTitleId);
        }
    }
}
