using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerStoreWebApi.Models;
using ComputerStoreWebApi.Repositories;

namespace ComputerStoreWebApi.Services
{
    public class DiscountService
    {
        private readonly IProductRepository _productRepository;

        public DiscountService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<(decimal total, string error)> CalculateDiscountAsync(BasketDto basket)
        {
            decimal total = 0;
            var productStock = new Dictionary<int, int>();
            var productCategoryCount = new Dictionary<int, int>();

            foreach (var item in basket.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    return (0, $"Product with ID {item.ProductId} not found.");
                }

                if (product.Quantity < item.Quantity)
                {
                    return (0, $"Not enough stock for product {product.Name}.");
                }

                if (!productStock.ContainsKey(product.Id))
                {
                    productStock[product.Id] = product.Quantity;
                }

                if (!productCategoryCount.ContainsKey(product.Categories.First().Id))
                {
                    productCategoryCount[product.Categories.First().Id] = 0;
                }

                productCategoryCount[product.Categories.First().Id] += item.Quantity;
            }

            foreach (var item in basket.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                var categoryId = product.Categories.First().Id;
                var quantity = item.Quantity;

                if (productCategoryCount[categoryId] > 1)
                {
                    total += product.Price * (quantity - 1) * 0.95m; // 5% discount 
                    total += product.Price; // Full price 
                }
                else
                {
                    total += product.Price * quantity; // No discount 
                }
            }

            return (total, null);
        }
    }
}
