using Bogus;
using DataAccess.DTO;

namespace DataAccess.Repositories.Seeders;

public class ProductSeeder(IProductRepository products)
{
    public async Task SeedProducts(int amount)
    {
        var productFaker = new Faker<NewProductDTO>();
        productFaker.RuleFor(o => o.Name, (f, u) => f.Commerce.ProductName());
        productFaker.RuleFor(o => o.Price, (f, u) => f.Random.Double(0, 1000));
        productFaker.RuleFor(o => o.LongDescription, (f, u) => f.Commerce.ProductDescription());
        productFaker.RuleFor(o => o.ShortDescription, (f, u) => f.Commerce.ProductDescription());
        productFaker.RuleFor(o => o.Listed, (f, u) => f.Random.Bool());
        productFaker.RuleFor(o => o.Stock, (f, u) => f.Random.Int(0, 100));

        foreach (var p in productFaker.GenerateLazy(amount))
        {
            await products.InsertProduct(p);
        }
    }
}