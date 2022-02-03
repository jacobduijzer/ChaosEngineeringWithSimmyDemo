using Refit;

namespace ChaosApi.WebApi;

public interface IBreweryDbApi
{
   [Get("/breweries")]
   Task<IEnumerable<Brewery>> Breweries();
}