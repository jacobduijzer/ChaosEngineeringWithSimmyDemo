namespace ChaosApi.WebApi.Products;

public class FakeProductService : IProductService
{
    public async Task<Product> ProductDetails(int productId) =>
        await Task.FromResult(_fakeProducts.First(x => x.ProductId.Equals(productId)));

    public async Task<List<Product>> UpsalesForProduct(int productId) =>
        await Task.FromResult(
            _upsales
                .First(with => with.Key.Equals(productId))
                .Value.Select(book => _fakeProducts.First(x => x.ProductId.Equals(book)))
                .ToList());

    private List<Product> _fakeProducts = new ()
    {
        new Product(1, "Agile Software Development with C# Book II Second Edition", 77.90m),
        new Product(2, "Unity in Action: Multiplatform game development in C#", 29.990m),
        new Product(3, "C#: Programming Basics for Absolute Beginners (Step-By-Step C#)", 24.95m),
        new Product(4, "Learn C# in One Day and Learn It Well: C# for Beginners with Hands-on Project (Learn Coding Fast with Hands-On Project)", 10.39m),
        new Product(5, "Code That Fits in Your Head: Heuristics for Software Engineering (Robert C. Martin Series)", 32.05m),
        new Product(6, "Agile Principles, Patterns, and Practices in C#", 29.990m),
        new Product(7, "Stylish F#: Crafting Elegant Functional Code for .NET and .NET Core", 35.000m),
        new Product(8, "Get Programming with F#: A guide for .NET developers", 27.95m),
    };

    private Dictionary<int, int[]> _upsales = new ()
    {
        [1] = new [] { 5, 6},
        [2] = new [] { 3, 4},
        [3] = new [] { 2, 4},
        [4] = new [] { 2, 3},
        [5] = new [] { 1, 6},
        [6] = new [] { 1, 5},
        [7] = new [] { 8 },
        [8] = new [] { 7 },
    };
}