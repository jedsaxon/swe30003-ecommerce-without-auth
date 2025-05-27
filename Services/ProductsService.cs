using DataAccess.Repositories;
using Domain;
using Microsoft.Extensions.Logging;

namespace Services;

public class ProductsService(IProductRepository productRepository)
{
    public async Task<Product[]> GetAllProducts()
    {
        return (await productRepository
            .GetProducts())
            .Select(s => Product.ProductWithIdentity(
                s.ProductId,
                s.Name,
                s.ShortDescription,
                s.LongDescription,
                (decimal)s.Price
            ))
            .ToArray();
    }

    public Task CreateProduct(string name, string shortDescription, string longDescription, double price)
    {
        return Task.CompletedTask;
    }
}