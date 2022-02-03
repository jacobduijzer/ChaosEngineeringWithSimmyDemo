using Microsoft.AspNetCore.Mvc;
using Polly;

namespace ChaosApi.WebApi.Controllers;

public class BreweryController : ControllerBase
{
    [HttpGet]
    [Route("/breweries")]
    public async Task<IActionResult> SearchBreweries(
        [FromServices] IBreweryDbApi api)
    {
        var breweries = await api.Breweries().ConfigureAwait(false);
        return new OkObjectResult(breweries);
    }

    [HttpGet]
    [Route("/resilientbreweries")]
    public async Task<IActionResult> GetBreweries([FromServices] IBreweryDbApi api, [FromServices]ILogger<BreweryController> logger)
    {
        var breweries = await Policy
            .Handle<HttpRequestException>()
            .RetryAsync(3, (exception, retryCount) => logger.LogInformation($"Retry {retryCount}/3: {exception.Message}"))
            .ExecuteAsync(async () => await api.Breweries());

        return new OkObjectResult(breweries);
    }
}
