using DataAccess.DTO;

namespace DataAccess.Repositories;

public interface IProductRepository
{
    public Task<List<ProductDTO>> GetProducts();
    public Task<ProductDTO?> GetById(Guid productId);

    /// <summary>
    /// Inserts a product into the database. Returns a new product with the set id if
    /// it successfully inserts. Otherwise, null.
    /// </summary>
    public Task<ProductDTO?> InsertProduct(NewProductDTO newProduct);

    /// <summary>
    /// Attempts to update the product. Returns whether the operation was successful.
    /// (Or really if a single record was updated)
    /// </summary>
    public Task<bool> UpdateProduct(ProductDTO toUpdate);

    Task DeleteProduct(Guid productId);
}