namespace ChaosApi.WebApi.Products;

public interface IProductService
{
    Task<Product> ProductDetails(int productId);

    Task<List<Product>> UpsalesForProduct(int productId);
}