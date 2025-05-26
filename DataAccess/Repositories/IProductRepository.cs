using DataAccess.DTO;

namespace DataAccess.Repositories;

public interface IProductRepository
{
    public Task<List<ProductDTO>> GetProducts();
    public Task<ProductDTO?> GetById(Guid productId);
    public Task<ProductDTO?> InsertProduct(NewProductDTO newProduct);
    public Task UpdateProduct(ProductDTO toUpdate);
}