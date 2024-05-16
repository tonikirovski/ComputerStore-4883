using System.Collections.Generic;
using System.Threading.Tasks;
using ComputerStoreWebApi.Data;
using ComputerStoreWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ComputerStoreWebApi.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;

        public CategoryRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                var existingCategory = await _context.Categories.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == category.Id);
                if (existingCategory == null)
                {
                    throw new KeyNotFoundException("Category not found.");
                }

                _context.Entry(category).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    throw new KeyNotFoundException("Category not found. It may have been deleted.");
                }
                else
                {
                    var databaseValues = (Category)databaseEntry.ToObject();
                    throw new DbUpdateConcurrencyException(
                        $"The category with ID {category.Id} was updated by another user. Current database values: " +
                        $"Name: {databaseValues.Name}, Description: {databaseValues.Description}");
                }
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
