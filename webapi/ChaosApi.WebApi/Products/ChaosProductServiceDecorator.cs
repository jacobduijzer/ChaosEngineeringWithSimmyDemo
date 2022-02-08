using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

namespace ChaosApi.WebApi.Products;

public class ChaosProductServiceDecorator : IProductService
{
    private readonly IProductService _inner;
    private readonly AsyncInjectOutcomePolicy _asyncChaosPolicy;

    public ChaosProductServiceDecorator(IProductService inner)
    {
        _inner = inner;
        _asyncChaosPolicy = MonkeyPolicy
            .InjectExceptionAsync((with) => with.Fault(new InvalidDataException("Chaos Monkey says Hi!"))
                .InjectionRate(0.5)
                .Enabled());
    }
    
    public async Task<Product> ProductDetails(int productId) =>
        await _inner.ProductDetails(productId);

    public async Task<List<Product>> UpsalesForProduct(int productId) =>
        await _asyncChaosPolicy
            .ExecuteAsync(async () => await _inner.UpsalesForProduct(productId));
}