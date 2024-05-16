using System.Collections.Generic;
using System.Threading.Tasks;
using ComputerStoreWebApi.Data;
using ComputerStoreWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ComputerStoreWebApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Categories).ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Categories)
                                          .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.Include(p => p.Categories)
                                                         .FirstOrDefaultAsync(p => p.Id == product.Id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Quantity = product.Quantity;

            // Update the categories
            existingProduct.Categories.Clear();
            foreach (var category in product.Categories)
            {
                var existingCategory = await _context.Categories.FindAsync(category.Id);
                if (existingCategory != null)
                {
                    existingProduct.Categories.Add(existingCategory);
                }
            }

            await _context.SaveChangesAsync();
        }


        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
