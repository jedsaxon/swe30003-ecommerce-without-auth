using DataAccess.Repositories;
using DataAccess.Repositories.Seeders;
using DataAccess.Repositories.Sqlite;

namespace WebApplication1;

public static class ApplicationBuilderExtensions
{
    public static async Task InitTables(this IApplicationBuilder builder)
    {
        await using var scope = builder.ApplicationServices.CreateAsyncScope();

        var db = scope.ServiceProvider.GetService<SqliteDataAccess>();
        if (db is null) throw new NullReferenceException("You did not provide the SqliteDataAccess service");

        var products = scope.ServiceProvider.GetService<IProductRepository>();
        if (products is null) throw new NullReferenceException("You did not provide a IProductRepository service");

        var currentProducts = await products.GetProducts();

        if (currentProducts.Count == 0)
        {
            var productSeeder = new ProductSeeder(products);
            await productSeeder.SeedProducts(100000);
        }

        await db.InitTablesAsync();
    }
}