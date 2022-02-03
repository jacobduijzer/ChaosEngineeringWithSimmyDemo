using Microsoft.AspNetCore.Mvc;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Behavior;
using Polly.Contrib.Simmy.Outcomes;

namespace ChaosApi.WebApi.Controllers;

public class UnstableController : ControllerBase
{
    private readonly InjectOutcomePolicy<BadHttpRequestException> _faultPolicy;
    private readonly InjectBehaviourPolicy _behaviourPolicy;

    public UnstableController()
    {
        _behaviourPolicy = MonkeyPolicy.InjectBehaviour(with =>
            with.Behaviour(() => throw new BadHttpRequestException("ChaosMonkey says whoeps"))
                .InjectionRate(0.55)
                .Enabled(true));
    }

    [HttpGet]
    [Route("/GetSomeData")]
    public ActionResult GetSomeData()
    {
        try
        {
            // run some unstable process
            _behaviourPolicy.Execute(() => true); 
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }

        return new OkObjectResult("some data");
    }
}