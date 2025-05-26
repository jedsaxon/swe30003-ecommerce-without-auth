using DataAccess.DTO;

namespace DataAccess.Repositories.Sqlite;

public class SqliteProductsRepository : IProductRepository
{
    public Task<List<ProductDTO>> GetProducts()
    {
        throw new NotImplementedException();
    }

    public Task<ProductDTO?> GetById(Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<ProductDTO?> InsertProduct(NewProductDTO newProduct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateProduct(ProductDTO toUpdate)
    {
        throw new NotImplementedException();
    }
}