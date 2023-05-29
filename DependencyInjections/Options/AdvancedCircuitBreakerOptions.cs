using System.ComponentModel.DataAnnotations;

namespace poc_circuit_break_providers.DependencyInjections.Options;

public record AdvancedCircuitBreakerOptions
{
    [Required]
    public double FailurePercentage { get; init; }

    [Required]
    public TimeSpan SamplingDuration { get; init; }

    [Required]
    public int MinumumSampling { get; init; }
}
