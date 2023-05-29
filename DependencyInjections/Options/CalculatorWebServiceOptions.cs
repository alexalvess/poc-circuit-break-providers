using System.ComponentModel.DataAnnotations;

namespace poc_circuit_break_providers.DependencyInjections.Options;

public record CalculatorWebServiceOptions
{
    [Required, Url]
    public string Endpoint { get; init; }

    [Required]
    public bool AllowCookies { get; init; }

    [Required]
    public TimeSpan SendTimeout { get; init; }

    [Required]
    public TimeSpan ReceiveTimeout { get; init; }

    [Required]
    public RetryPolicyOptions RetryPolicyOptions { get; set; }
}
