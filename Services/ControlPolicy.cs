using Microsoft.Extensions.Options;
using poc_circuit_break_providers.DependencyInjections.Options;
using Polly;

namespace poc_circuit_break_providers.Services;

public class ControlPolicy
{
	public ControlPolicy(IOptionsMonitor<CalculatorWebServiceOptions> options)
	{
        PolicyWrap = Policy.WrapAsync(
            ProviderPolicy.GetRetryPolicyAsync(options.CurrentValue.RetryPolicyOptions),
            ProviderPolicy.GetAdvancedCircuitBreakerPolicyAsync(options.CurrentValue.CircuitBreakerOptions));
    }

    public AsyncPolicy PolicyWrap { get; }
}
