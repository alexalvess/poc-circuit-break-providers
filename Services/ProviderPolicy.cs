using Polly.Timeout;
using Polly;
using Polly.CircuitBreaker;
using System.Diagnostics;
using poc_circuit_break_providers.DependencyInjections.Options;

namespace poc_circuit_break_providers.Services;

public static class ProviderPolicy
{
    public static IAsyncPolicy GetRetryPolicyAsync(RetryPolicyOptions options)
        => Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: options.RetryCount,
                sleepDurationProvider: retryAttempt => options.SleepDurationPower,
                onRetry: (exception, timespan, count, _)
                    => LogError($"Retrying in {timespan.TotalSeconds}s. Attempt: {count}. Message: {exception.Message}"))
            .WrapAsync(
                Policy.TimeoutAsync(
                    seconds: options.EachRetryTimeout,
                    timeoutStrategy: TimeoutStrategy.Optimistic,
                    onTimeoutAsync: async (_, timeout, _) => { await Task.Yield(); LogError($"Timeout applied after {timeout}s"); }));

    public static IAsyncPolicy GetAdvancedCircuitBreakerPolicyAsync(CircuitBreakerOptions options)
        => Policy
            .Handle<Exception>()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: options.AdvancedCircuitBreakerOptions.FailurePercentage,
                samplingDuration: options.AdvancedCircuitBreakerOptions.SamplingDuration,
                minimumThroughput: options.AdvancedCircuitBreakerOptions.MinumumSampling,
                durationOfBreak: options.DurationOfBreak,
                onBreak: (_, state, breakingTime, _) => LogWarning($"Circuit breaking! State: {state}. Break time: {breakingTime.TotalSeconds}s"),
                onReset: _ => LogWarning($"Circuit resetting!"),
                onHalfOpen: () => LogWarning($"Circuit transitioning to {CircuitState.HalfOpen}"));

    private static void LogError(string message)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine(message.Length > 100 ? message[..100] : message);
        Console.ResetColor();
    }

    private static void LogWarning(string message)
    {
        Console.BackgroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(message.Length > 100 ? message[..100] : message);
        Console.ResetColor();
    }
}