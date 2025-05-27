using DataAccess.DTO;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories.Sqlite;

public class SqliteProductsRepository(SqliteDataAccess dataAccess, ILogger<SqliteProductsRepository> logger) : IProductRepository
{
    public async Task<List<ProductDTO>> GetProducts()
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              select id, name, short_description, long_description, price
                              from products;
                              """;

        await using var reader = await command.ExecuteReaderAsync();
        var results = new List<ProductDTO>();

        while (await reader.ReadAsync())
        {
            results.Add(new ProductDTO(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetDouble(4)
            ));
        }

        return results;
    }

    public async Task<ProductDTO?> GetById(Guid productId)
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              select id, name, short_description, long_description, price
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
            reader.GetDouble(4));
    }

    public async Task<ProductDTO?> InsertProduct(NewProductDTO newProduct)
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              insert into products (id, name, short_description, long_description, price)
                              values (:id, :name, :short_description, :long_description, :price);
                              """;

        var newId = Guid.NewGuid();

        command.Parameters.AddWithValue(":id", newId.ToString());
        command.Parameters.AddWithValue(":name", newProduct.Name);
        command.Parameters.AddWithValue(":short_description", newProduct.ShortDescription);
        command.Parameters.AddWithValue(":long_description", newProduct.LongDescription);
        command.Parameters.AddWithValue(":price", newProduct.Price);

        if (await command.ExecuteNonQueryAsync() == 0)
            return null;

        return new ProductDTO(
            newId,
            newProduct.Name,
            newProduct.ShortDescription,
            newProduct.LongDescription,
            newProduct.Price
        );
    }

    public async Task<bool> UpdateProduct(ProductDTO toUpdate)
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              update products
                              set name=:name, short_description=:short_description, long_description=:long_description, price=:price
                              where id=:id
                              """;
        command.Parameters.AddWithValue(":id", toUpdate.ProductId);
        command.Parameters.AddWithValue(":short_description", toUpdate.ShortDescription);
        command.Parameters.AddWithValue(":long_description", toUpdate.LongDescription);
        command.Parameters.AddWithValue(":price", toUpdate.Price);
        command.Parameters.AddWithValue(":name", toUpdate.Name);

        return await command.ExecuteNonQueryAsync() == 1;
    }
}