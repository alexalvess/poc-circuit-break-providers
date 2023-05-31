using Microsoft.Extensions.Options;
using poc_circuit_break_providers.DependencyInjections.Options;
using Polly;

namespace poc_circuit_break_providers.Services.WebServices;

public class ControlPolicy
{
    public ControlPolicy(IOptionsMonitor<CalculatorWebServiceOptions> options)
    {
        PolicyWrap = Policy.WrapAsync(
            WcfPolicy.GetRetryPolicyAsync(options.CurrentValue.RetryPolicyOptions),
            WcfPolicy.GetAdvancedCircuitBreakerPolicyAsync(options.CurrentValue.CircuitBreakerOptions));
    }

    public AsyncPolicy PolicyWrap { get; }
}
