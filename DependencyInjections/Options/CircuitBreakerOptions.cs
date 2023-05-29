using System.ComponentModel.DataAnnotations;

namespace poc_circuit_break_providers.DependencyInjections.Options;

public class CircuitBreakerOptions
{
    [Required]
    public int CircuitBreaking { get; init; }

    [Required]
    public TimeSpan DurationOfBreak { get; init; }

    [Required]
    public AdvancedCircuitBreakerOptions AdvancedCircuitBreakerOptions { get; set; }
}
