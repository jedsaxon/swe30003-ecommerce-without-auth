using DataAccess.DTO;
using DataAccess.Repositories;
using Domain;
using Microsoft.Extensions.Logging;

namespace Services;

public class ProductsService(IProductRepository productRepository)
{
    public async Task<Product[]> GetAllProducts(bool includeUnlisted = false)
    {
        return (await productRepository
            .GetProducts(includeUnlisted))
            .Select(s => Product.ProductWithIdentity(
                s.ProductId,
                s.Name,
                s.ShortDescription,
                s.LongDescription,
                (decimal)s.Price,
                s.Listed,
                s.Stock
            ))
            .ToArray();
    }

    public async Task CreateProduct(string name, string shortDescription, string longDescription, double price, bool listed, int stock)
    {
        await productRepository.InsertProduct(new NewProductDTO(name, shortDescription, longDescription, price, listed, stock));
    }

    public async Task<Product?> FindProductById(Guid id)
    {
        var found = await productRepository.GetById(id);
        if (found is null) return null;

        return Product.ProductWithIdentity(found.ProductId, found.Name, found.ShortDescription, found.LongDescription, (decimal)found.Price, found.Listed, found.Stock);
    }

    public async Task<bool> EditProduct(Guid productId, string name, string shortDescription, string longDescription,
        double price, bool listed, int stock)
    {
        var productDto = await productRepository.GetById(productId);
        if (productDto is null)
            throw new ArgumentException("This product does not exist", nameof(productId));

        var product = Product.ProductWithIdentity(productDto.ProductId, productDto.Name, productDto.ShortDescription,
            productDto.LongDescription, (decimal)productDto.Price, productDto.Listed, productDto.Stock);

        if (product.Id == null)
            throw new NullReferenceException("Product ID, despite having come from IProductRepository, is null");

        product.Rename(name);
        product.SetShortDescription(shortDescription);
        product.SetLongDescription(longDescription);
        product.UpdatePrice(price);
        if (listed) product.Enlist();
        else product.Unlist();
        product.SetStock(stock);

        return await productRepository.UpdateProduct(new ProductDTO((Guid)product.Id, product.Name, product.ShortDescription,
            product.LongDescription, (double)product.Price, product.Listed, product.Stock));
    }

    /// <returns>Whether the operation was successful</returns>
    public async Task<bool> DeleteProduct(Guid productId)
    {
        var product = await FindProductById(productId);
        if (product is null) return false;

        await productRepository.DeleteProduct(productId);
        return true;
    }
}