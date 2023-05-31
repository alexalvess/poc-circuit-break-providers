namespace poc_circuit_break_providers.Services.WebServices.Proxies;

public interface ICalculatorProxy : ICalculatorService
{
    Task<int> AddWithoutCircuitAsync(int intA, int intB);
}
