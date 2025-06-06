using System.Diagnostics;
using DataAccess.DTO;
using DataAccess.Repositories;
using DataAccess.Repositories.Seeders;
using DataAccess.Repositories.Sqlite;
using Services;

namespace WebApplication1;

public static class ApplicationBuilderExtensions
{
    public static async Task InitTables(this IApplicationBuilder builder)
    {
        await using var scope = builder.ApplicationServices.CreateAsyncScope();

        var db = scope.ServiceProvider.GetService<SqliteDataAccess>();
        if (db is null) throw new NullReferenceException("You did not provide the SqliteDataAccess service");

        await db.InitTablesAsync();
    }

    public static async Task SeedDatabase(this IApplicationBuilder builder)
    {
        await using var scope = builder.ApplicationServices.CreateAsyncScope();
        
        var db = scope.ServiceProvider.GetService<SqliteDataAccess>();
        if (db is null) throw new NullReferenceException("You did not provide the SqliteDataAccess service");

        var products = scope.ServiceProvider.GetService<IProductRepository>();
        if (products is null) throw new NullReferenceException("You did not provide a IProductRepository service");
        
        var currentProducts = await products.GetProducts(true);

        if (currentProducts.Count == 0)
        {
            var productSeeder = new ProductSeeder(products);
            await productSeeder.SeedProducts(100);
        }
    }

    /// <summary>
    /// If an administrator account does not yet exist in the database, it will create a new one. It's password will be randomly
    /// generated and logged into the console. It is expected that the administrator will reset their password once they access
    /// the site.
    /// </summary>
    public static async Task InitAdminAccount(this IApplicationBuilder builder)
    {
        await using var scope = builder.ApplicationServices.CreateAsyncScope();

        var users = scope.ServiceProvider.GetService<UserService>();
        Debug.Assert(users is not null, "You did not provide the UserService service");

        var configuration = scope.ServiceProvider.GetService<IConfiguration>();
        Debug.Assert(configuration is not null, "You did not provide the Configuration service");

        var adminEmail = configuration["DefaultAdminEmail"] ??
                         throw new NullReferenceException("You must provide the DefaultAdminEmail configuration option.");

        var user = await users.FindUserByEmail(adminEmail);

        if (user is null)
        {
            var newPassword = Guid.NewGuid();
            await users.CreateAdminAccount("Admin", "Admin", newPassword.ToString(), adminEmail, "0000000000");
            Console.WriteLine($"Admin Account Created. Please log in and change the password to avoid account this being compromise. " +
                                  $"{'{'} Email = {adminEmail}, Password: {newPassword} {'}'};");
        }
        else
        {
            Console.WriteLine("Admin account already exists.");
        }
    }
}