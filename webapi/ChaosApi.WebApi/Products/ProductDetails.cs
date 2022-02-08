namespace ChaosApi.WebApi.Products;

public record ProductDetails(Product Product, IEnumerable<Product>? Upsales);