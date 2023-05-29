using System.ComponentModel.DataAnnotations;

namespace poc_circuit_break_providers.DependencyInjections.Options;

public record RetryPolicyOptions
{
    [Required]
    public int RetryCount { get; init; }

    [Required]
    public TimeSpan SleepDurationPower { get; init; }

    [Required]
    public int EachRetryTimeout { get; init; }
}
