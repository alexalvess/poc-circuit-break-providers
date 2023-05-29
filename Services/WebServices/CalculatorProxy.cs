using Microsoft.Extensions.Options;
using poc_circuit_break_providers.DependencyInjections.Options;
using Polly;
using Polly.Wrap;
using System.ServiceModel;

namespace poc_circuit_break_providers.Services.WebServices;

public class CalculatorProxy : ICalculatorProxy
{
    private readonly ICalculatorService _service;
	private readonly AsyncPolicyWrap _policyWrap;

	public CalculatorProxy(ICalculatorService service, IOptionsMonitor<CalculatorWebServiceOptions> options)
	{
		_policyWrap = Policy.WrapAsync(
            ProviderPolicy.GetRetryPolicyAsync(
                retryCount: options.CurrentValue.RetryCount, 
                sleepDurationPower: options.CurrentValue.SleepDurationPower, 
                eachRetryTimeout: options.CurrentValue.EachRetryTimeout),
            ProviderPolicy.GetAdvancedCircuitBreakerPolicyAsync(
                circuitBreaking: options.CurrentValue.CircuitBreaking, 
                durationOfBreak: options.CurrentValue.DurationOfBreak));

		_service = service;
	}

    public Task<int> AddAsync(int intA, int intB)
        => _policyWrap.ExecuteAsync(() => _service.AddAsync(intA, intB));

    public Task<int> DivideAsync(int intA, int intB)
        => _policyWrap.ExecuteAsync(() => _service.DivideAsync(intA, intB));

    public Task<int> MultiplyAsync(int intA, int intB)
        => _policyWrap.ExecuteAsync(() => _service.MultiplyAsync(intA, intB));

    public Task<int> SubtractAsync(int intA, int intB)
        => _policyWrap.ExecuteAsync(() => _service.SubtractAsync(intA, intB));

    /*
    30seg
    10req
    70% - circuit open

    duration - 5min

    qtd - 6
     */
}
