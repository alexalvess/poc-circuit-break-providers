using Microsoft.Extensions.Options;
using poc_circuit_break_providers.DependencyInjections.Options;
using Polly;

namespace poc_circuit_break_providers.Services.WebServices;

public class WcfControlPolicy
{
    public WcfControlPolicy(IOptionsMonitor<CalculatorWebServiceOptions> options)
    {
        PolicyWrap = Policy.WrapAsync(
            WcfPolicy.GetRetryPolicyAsync(options.CurrentValue.RetryPolicyOptions),
            WcfPolicy.GetAdvancedCircuitBreakerPolicyAsync(options.CurrentValue.CircuitBreakerOptions));
    }

    public AsyncPolicy PolicyWrap { get; }
}
