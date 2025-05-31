using DataAccess.DTO;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories.Sqlite;

public class SqliteProductsRepository(SqliteDataAccess dataAccess, ILogger<SqliteProductsRepository> logger) : IProductRepository
{
    public async Task<List<ProductDTO>> GetProducts(bool includeUnlisted)
    {
        var command = await dataAccess.CreateCommand();

        var commandText = """
                          select id, name, short_description, long_description, price, listed
                          from products
                          """;
        // If we don't include unlisted products, then only return those that are listed
        // Maybe I should just rename this argument!
        if (!includeUnlisted) commandText += " where listed = true";
        command.CommandText = commandText;

        await using var reader = await command.ExecuteReaderAsync();
        var results = new List<ProductDTO>();

        while (await reader.ReadAsync())
        {
            results.Add(new ProductDTO(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetDouble(4),
                reader.GetBoolean(5)
            ));
        }

        return results;
    }

    public async Task<ProductDTO?> GetById(Guid productId)
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              select id, name, short_description, long_description, price, listed
                              from products
                              where id = :id
                              """;
        command.Parameters.AddWithValue(":id", productId.ToString());

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new ProductDTO(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetDouble(4),
            reader.GetBoolean(5));
    }

    public async Task<ProductDTO?> InsertProduct(NewProductDTO newProduct)
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              insert into products (id, name, short_description, long_description, price, listed)
                              values (:id, :name, :short_description, :long_description, :price, :listed);
                              """;

        var newId = Guid.NewGuid();

        command.Parameters.AddWithValue(":id", newId.ToString());
        command.Parameters.AddWithValue(":name", newProduct.Name);
        command.Parameters.AddWithValue(":short_description", newProduct.ShortDescription);
        command.Parameters.AddWithValue(":long_description", newProduct.LongDescription);
        command.Parameters.AddWithValue(":price", newProduct.Price);
        command.Parameters.AddWithValue(":listed", newProduct.Listed);

        if (await command.ExecuteNonQueryAsync() == 0)
            return null;

        return new ProductDTO(
            newId,
            newProduct.Name,
            newProduct.ShortDescription,
            newProduct.LongDescription,
            newProduct.Price,
            newProduct.Listed
        );
    }

    public async Task<bool> UpdateProduct(ProductDTO toUpdate)
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              update products
                              set name=:name, short_description=:short_description, long_description=:long_description, price=:price, listed=:listed
                              where id=:id
                              """;
        command.Parameters.AddWithValue(":id", toUpdate.ProductId.ToString());
        command.Parameters.AddWithValue(":short_description", toUpdate.ShortDescription);
        command.Parameters.AddWithValue(":long_description", toUpdate.LongDescription);
        command.Parameters.AddWithValue(":price", toUpdate.Price);
        command.Parameters.AddWithValue(":name", toUpdate.Name);
        command.Parameters.AddWithValue(":listed", toUpdate.Listed);

        return await command.ExecuteNonQueryAsync() == 1;
    }

    public async Task DeleteProduct(Guid productId)
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              delete from products
                              where id = :id
                              """;
        command.Parameters.AddWithValue(":id", productId.ToString());
        await command.ExecuteNonQueryAsync();
    }
}