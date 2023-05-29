﻿using Polly.Timeout;
using Polly;
using Polly.CircuitBreaker;
using System.Diagnostics;
using poc_circuit_break_providers.DependencyInjections.Options;

namespace poc_circuit_break_providers.Services;

public static class ProviderPolicy
{
    public static ISyncPolicy GetRetryPolicy(RetryPolicyOptions options)
        => Policy
            .Handle<Exception>()
            .WaitAndRetry(
                retryCount: options.RetryCount,
                sleepDurationProvider: retryAttempt => options.SleepDurationPower,
                onRetry: (exception, timespan)
                    => LogError($"Retrying in {timespan.TotalSeconds}s. Attempt: {options.RetryCount}. Message: {exception.Message}"))
            .Wrap(
                Policy.Timeout(
                    seconds: options.EachRetryTimeout,
                    timeoutStrategy: TimeoutStrategy.Optimistic,
                    onTimeout: (_, timeout, _) => { LogError($"Timeout applied after {timeout}s"); }));

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

    public static ISyncPolicy GetCircuitBreakerPolicy(int circuitBreaking, TimeSpan durationOfBreak)
        => Policy
            .Handle<Exception>()
            .CircuitBreaker(
                exceptionsAllowedBeforeBreaking: circuitBreaking,
                durationOfBreak: durationOfBreak,
                onBreak: (_, state, breakingTime, _) => LogWarning($"Circuit breaking! State: {state}. Break time: {breakingTime.TotalSeconds}s"),
                onReset: _ => LogWarning("Circuit resetting!"),
                onHalfOpen: () => LogWarning($"Circuit transitioning to {CircuitState.HalfOpen}"));

    public static IAsyncPolicy GetCircuitBreakerPolicyAsync(int circuitBreaking, TimeSpan durationOfBreak)
        => Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: circuitBreaking,
                durationOfBreak: durationOfBreak,
                onBreak: (_, state, breakingTime, _) => LogWarning($"Circuit breaking! State: {state}. Break time: {breakingTime.TotalSeconds}s"),
                onReset: _ => LogWarning("Circuit resetting!"),
                onHalfOpen: () => LogWarning($"Circuit transitioning to {CircuitState.HalfOpen}"));

    public static IAsyncPolicy GetAdvancedCircuitBreakerPolicyAsync(
        double failurePercentage, 
        TimeSpan samplingDuration, 
        int minumumSampling,
        TimeSpan durationOfBreak)
        => Policy
            .Handle<Exception>()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: failurePercentage,
                samplingDuration: samplingDuration,
                minimumThroughput: minumumSampling,
                durationOfBreak: durationOfBreak,
                onBreak: (_, state, breakingTime, _) => LogWarning($"Circuit breaking! State: {state}. Break time: {breakingTime.TotalSeconds}s"),
                onReset: _ => LogWarning($"Circuit resetting!"),
                onHalfOpen: () => LogWarning($"Circuit transitioning to {CircuitState.HalfOpen}"));

    private static void LogError(string message)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine(message.Length > 100 ? message.Substring(0, 100) : message);
        Console.ResetColor();
    }

    private static void LogWarning(string message)
    {
        Console.BackgroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(message.Length > 100 ? message.Substring(0, 100) : message);
        Console.ResetColor();
    }
}