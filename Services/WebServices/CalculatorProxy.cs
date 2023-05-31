using Microsoft.Extensions.Options;
using poc_circuit_break_providers.DependencyInjections.Options;
using Polly;
using Polly.Wrap;
using System.ServiceModel;

namespace poc_circuit_break_providers.Services.WebServices;

public class CalculatorProxy : ICalculatorProxy
{
    private readonly ICalculatorService _service;
	private readonly WcfControlPolicy _controlPolicy;

	public CalculatorProxy(ICalculatorService service, WcfControlPolicy controlPolicy)
	{
		_service = service;
        _controlPolicy = controlPolicy;
	}

    public Task<int> AddWithoutCircuitAsync(int intA, int intB)
        => _service.AddAsync(intA, intB);

    public Task<int> AddAsync(int intA, int intB)
        => _controlPolicy.PolicyWrap.ExecuteAsync(() => _service.AddAsync(intA, intB));

    public Task<int> DivideAsync(int intA, int intB)
        => _controlPolicy.PolicyWrap.ExecuteAsync(() => _service.DivideAsync(intA, intB));

    public Task<int> MultiplyAsync(int intA, int intB)
        => _controlPolicy.PolicyWrap.ExecuteAsync(() => _service.MultiplyAsync(intA, intB));

    public Task<int> SubtractAsync(int intA, int intB)
        => _controlPolicy.PolicyWrap.ExecuteAsync(() => _service.SubtractAsync(intA, intB));
}