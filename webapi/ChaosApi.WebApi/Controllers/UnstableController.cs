using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Behavior;
using Polly.Contrib.Simmy.Outcomes;

namespace ChaosApi.WebApi.Controllers;

public class UnstableController : ControllerBase
{
    private readonly InjectOutcomePolicy<BadHttpRequestException> _faultPolicy;
    private readonly InjectBehaviourPolicy _behaviourPolicy;
    private readonly InjectOutcomePolicy<IActionResult> _chaosPolicy;

    public UnstableController()
    {
        _behaviourPolicy = MonkeyPolicy.InjectBehaviour(with =>
            with.Behaviour(() => throw new BadHttpRequestException("Chaos Monkey says whoops!"))
                .InjectionRate(0.5)
                .Enabled(true));

        var result = new OkObjectResult(DateTime.UtcNow.AddYears(-3).ToString(CultureInfo.InvariantCulture));
        _chaosPolicy = MonkeyPolicy.InjectResult<IActionResult>(with =>
            with.Result(result)
                .InjectionRate(0.5)
                .Enabled()
        );
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

    [HttpGet]
    [Route("CurrentDate")]
    public IActionResult GetCurrentDate()
    {
        return _chaosPolicy.Execute(() => new OkObjectResult(DateTime.Now.ToString(CultureInfo.InvariantCulture)));
    }
}