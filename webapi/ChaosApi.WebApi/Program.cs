using ChaosApi.WebApi;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Outcomes;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var chaosSettings = builder.Configuration.GetSection("SimmySettings").Get<SimmyConfigurePolicies.FaultOptions>();

AsyncInjectOutcomePolicy<HttpResponseMessage> faultPolicy = MonkeyPolicy.InjectFaultAsync<HttpResponseMessage>(
    new HttpRequestException("Simmy threw an exception"), 
    injectionRate: chaosSettings.FaultPolicySettings.InjectionRate,
    enabled: () => chaosSettings.FaultPolicySettings.Enabled
);

AsyncInjectLatencyPolicy<HttpResponseMessage> latencyPolicy = MonkeyPolicy.InjectLatencyAsync<HttpResponseMessage>(
    TimeSpan.FromSeconds(chaosSettings.LatencyPolicySettings.Latency),
    chaosSettings.LatencyPolicySettings.InjectionRate,
    () => chaosSettings.LatencyPolicySettings.Enabled);

builder.Services.AddRefitClient<IBreweryDbApi>()
    .ConfigureHttpClient(config => config.BaseAddress = new Uri("https://api.openbrewerydb.org"))
    .AddPolicyHandler(faultPolicy)
    .AddPolicyHandler(latencyPolicy);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection().UseAuthorization();
app.MapControllers();
app.Run();