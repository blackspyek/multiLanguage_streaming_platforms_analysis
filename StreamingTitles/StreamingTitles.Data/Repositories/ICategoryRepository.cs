using StreamingTitles.Data.Model;

namespace StreamingTitles.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> DropCategoryAsync();
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryById(int categoryid);
        Task<Category> GetCategoryByName(string categoryName);

        Task<ICollection<Title>> GetTitlesByCategoryId(int categoryid);
        Task<bool> CreateCategory(Category category);
        Task<bool> UpdateCategory(Category category);
        Task<bool> DeleteCategory(Category category);
        Task<bool> Save();
        Task<bool> CategoryExists(int categoryid);
    }
}