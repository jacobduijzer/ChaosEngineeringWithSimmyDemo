using Polly;
using Polly.CircuitBreaker;

namespace ChaosApi.WebApi.Products;

public class ResilientProductServiceDecorator : IProductService
{
    private readonly IProductService _inner;
    private readonly ILogger<ResilientProductServiceDecorator> _logger;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

    public ResilientProductServiceDecorator(IProductService inner, ILogger<ResilientProductServiceDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
        
        _circuitBreakerPolicy = Policy
            .Handle<InvalidDataException>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10),
                (ex, t) => logger.LogInformation("Circuit broken!"),
                () => logger.LogInformation("Circuit reset!"));
    }

    public async Task<Product> ProductDetails(int productId) =>
        await _inner.ProductDetails(productId);

    public async Task<List<Product>> UpsalesForProduct(int productId)
    {
        var brokenPolicy = Policy<List<Product>>
            .Handle<BrokenCircuitException>()
            .FallbackAsync(async (cancellationToken) =>
            {
                _logger.LogInformation("Returning empty result, circuit is broken");
                return await Task.FromResult(new List<Product>());
            });
       
        var retryPolicy = Policy
            .Handle<InvalidDataException>()
            .WaitAndRetryAsync(retryCount: 2, sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(50),
                (exception, timeSpan, retryCount, context) => _logger.LogInformation($"Wait and retry ({retryCount}/{2}): {exception.Message}"));

        return await brokenPolicy.WrapAsync(retryPolicy).WrapAsync(_circuitBreakerPolicy)
            .ExecuteAsync(async () => 
            { 
                var upsaleProducts = await _inner.UpsalesForProduct(productId);
                _logger.LogInformation($"Returning data, {upsaleProducts.Count} upsale products found");
                return upsaleProducts;
            });
    }
}