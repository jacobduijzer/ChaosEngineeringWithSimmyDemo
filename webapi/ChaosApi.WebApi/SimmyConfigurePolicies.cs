namespace ChaosApi.WebApi;

public class SimmyConfigurePolicies
{
    public class FaultOptions
    {
        public LatencyPolicySettings LatencyPolicySettings { get; set; }
        public FaultPolicySettings FaultPolicySettings { get; set; }
    }

    public class LatencyPolicySettings
    {
        public bool Enabled { get; set; }
        public double Latency { get; set; }
        public double InjectionRate { get; set; }
    }

    public class FaultPolicySettings
    {
        public bool Enabled { get; set; }
        public double InjectionRate { get; set; }
    }
    
    public bool EnableProductServiceChaos { get; set; }
    public bool EnableResilientProductService { get; set; }
}