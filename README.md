# Chaos Engineering with Simmy

## Controller with external dependencies

The `BreweryController` calls an external API, the API of the [Open Brewery Database](openbrewerydb.org). The connection to the external service is behaving chaotic! There is also a Chaos Monkey scrambling the result once in a while!

### Testing

Use the `/breweries` endpoint in swagger to test the disfunctioning API calls.

Use the `/resilientbreweries` to test the same API but now with a retry policy. The result of this call will be much better!

Use the `/scrambledbreweries` to test the API with a misbehaving Behaviour Chaos Monkey which will scramble the data once in a while.

## Unstable Controller

Just to demonstrate some other chaos options, I added a `UnstableController`. Just look into the code and test it with swagger.

## Product controller with both Chaos Monkeys and Tamed Monkeys

The `ProductController` is using a `ProductService` which can be decorated with two decorators: a `ChaosProductServiceDecorator` which injects chaos into the `ProductService` and a `ResilientProductServiceDecorator` which uses policies to handle exceptions and create a resilient product service. To enable the decorators change the `appsettings.json`:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "SimmySettings": {
    "FaultPolicySettings": {
      "Enabled": true,
      "InjectionRate": 0.5
    },
    "LatencyPolicySettings": {
      "Enabled": true,
      "Latency": 3,
      "InjectionRate": 0.5
    },
    "EnableProductServiceChaos": false, // <- change this to true to get a chaotic experience
    "EnableResilientProductService": false // <- also change this to true to create a resilient service 
  }
}
```

### Testing the chaos

Use the swagger page to request product details. If everything is working well the result will be a book and a few related books. If something goes wrong an empty list of related books will be returned.



## Inspiration / Links / Resources


* [What is Simmy](http://www.thepollyproject.org/2019/06/27/simmy-the-monkey-for-making-chaos/)
* [Simmy (Github)](https://github.com/Polly-Contrib/Simmy)
* [Simmy with Azure App Configuration](http://elvanydev.com/simmy-with-azure-app-configuration/)
* [Kolton Andrus on Lessons Learnt From Failure Testing at Amazon and Netflix and New Venture Gremlin, InfoQ Podcast](https://www.infoq.com/podcasts/failure-as-a-service/)
* [Simmy and Chaos Engineering Geovanny Alzate Sandoval, Adventures In .Net Podcast](https://adventuresindotnet.com/13)
* [Chaos Engineering with Charles Torre, MS Dev Show Podcast](https://msdevshow.com/2016/11/chaos-engineering-with-charles-torre/)
