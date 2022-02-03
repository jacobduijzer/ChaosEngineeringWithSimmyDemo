using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Behavior;

namespace ChaosApi.WebApi.Controllers;

public class BreweryController : ControllerBase
{
    private bool _scrambleResult;
    private readonly InjectBehaviourPolicy _behaviourPolicy;

    public BreweryController()
    {
        _behaviourPolicy = MonkeyPolicy.InjectBehaviour(with =>
            with.Behaviour(() => _scrambleResult = true)
                .InjectionRate(0.75)
                .Enabled(true));
    }

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
    public async Task<IActionResult> GetBreweries([FromServices] IBreweryDbApi api,
        [FromServices] ILogger<BreweryController> logger)
    {
        var breweries = await Policy
            .Handle<HttpRequestException>()
            .RetryAsync(3, (exception, retryCount) => logger.LogInformation($"Retry {retryCount}/3: {exception.Message}"))
            .ExecuteAsync(async () => await api.Breweries());

        return new OkObjectResult(breweries);
    }

    [HttpGet]
    [Route("/scrambledbreweries")]
    public async Task<IActionResult> GetScrambledBreweries(
        [FromServices] IBreweryDbApi api)
    {
        var breweries = await api.Breweries().ConfigureAwait(false);
        var scrambledBreweries = breweries
            .Select(brewery => new Brewery
            {
                Id = Shuffle(brewery.Id),
                Name = Shuffle(brewery.Name), 
                City = Shuffle(brewery.City), 
                Country = Shuffle(brewery.City)
            })
            .ToList();
        return new OkObjectResult(_behaviourPolicy.Execute(() => _scrambleResult ? scrambledBreweries : breweries));
    }

    private static string Shuffle(string stringToShuffle)
    {
        var array = stringToShuffle.ToCharArray();
        Random rng = new();
        var n = array.Length;
        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            (array[k], array[n]) = (array[n], array[k]);
        }

        return new string(array);
    }
}