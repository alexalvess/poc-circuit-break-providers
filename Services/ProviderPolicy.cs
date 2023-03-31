using Polly.Timeout;
using Polly;
using Polly.CircuitBreaker;
using System.Diagnostics;

namespace poc_circuit_break_providers.Services;

public static class ProviderPolicy
{
    public static ISyncPolicy GetRetryPolicy(int retryCount, int sleepDurationPower, int eachRetryTimeout)
        => Policy
            .Handle<Exception>()
            .WaitAndRetry(
                retryCount: retryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(sleepDurationPower, retryAttempt)),
                onRetry: (exception, timespan)
                    => Console.WriteLine($"Retrying in {timespan.TotalSeconds}s. Attempt: {retryCount}. Message: {exception.Message}"))
            .Wrap(
                Policy.Timeout(
                    seconds: eachRetryTimeout,
                    timeoutStrategy: TimeoutStrategy.Optimistic,
                    onTimeout: (_, timeout, _) => { Console.WriteLine($"Timeout applied after {timeout}s"); }));

    public static IAsyncPolicy GetRetryPolicyAsync(int retryCount, int sleepDurationPower, int eachRetryTimeout)
        => Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: retryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(sleepDurationPower, retryAttempt)),
                onRetry: (exception, timespan, count, _)
                    => Console.WriteLine($"Retrying in {timespan.TotalSeconds}s. Attempt: {count}. Message: {exception.Message}"))
            .WrapAsync(
                Policy.TimeoutAsync(
                    seconds: eachRetryTimeout,
                    timeoutStrategy: TimeoutStrategy.Optimistic,
                    onTimeoutAsync: async (_, timeout, _) => { await Task.Yield(); Console.WriteLine($"Timeout applied after {timeout}s"); }));

    public static ISyncPolicy GetCircuitBreakerPolicy(int circuitBreaking, TimeSpan durationOfBreak)
        => Policy
            .Handle<Exception>()
            .CircuitBreaker(
                exceptionsAllowedBeforeBreaking: circuitBreaking,
                durationOfBreak: durationOfBreak,
                onBreak: (_, state, breakingTime, _) => Console.WriteLine($"Circuit breaking! State: {state}. Break time: {breakingTime.TotalSeconds}s"),
                onReset: _ => Console.WriteLine("Circuit resetting!"),
                onHalfOpen: () => Console.WriteLine($"Circuit transitioning to {CircuitState.HalfOpen}"));

    public static IAsyncPolicy GetCircuitBreakerPolicyAsync(int circuitBreaking, TimeSpan durationOfBreak)
        => Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: circuitBreaking,
                durationOfBreak: durationOfBreak,
                onBreak: (_, state, breakingTime, _) => Console.WriteLine($"Circuit breaking! State: {state}. Break time: {breakingTime.TotalSeconds}s"),
                onReset: _ => Console.WriteLine("Circuit resetting!"),
                onHalfOpen: () => Console.WriteLine($"Circuit transitioning to {CircuitState.HalfOpen}"));
}