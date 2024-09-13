using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;

using Moq;
using CurrencyConverter.Services;
using Polly.Timeout;
using Polly.CircuitBreaker;


[TestClass]
public class ResiliencePipelineProviderTests
{
    private ResiliencePipelineProvider _resiliencePipelineProvider;

    [TestInitialize]
    public void Setup()
    {
        _resiliencePipelineProvider = new ResiliencePipelineProvider();
    }

    [TestMethod]
    public async Task ShouldRetryOnFailure()
    {
        // Arrange: Set up the action to throw an exception the first time, then succeed
        var attempt = 0;
        Func<CancellationToken, Task<HttpResponseMessage>> action = (ct) =>
        {
            attempt++;
            if (attempt == 1)
            {
                throw new HttpRequestException("Simulated transient failure");
            }
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        };

        // Act
        var result = await _resiliencePipelineProvider.ExecuteAsync(action, CancellationToken.None);

        // Assert
        Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
        Assert.AreEqual(2, attempt, "Should retry once before succeeding.");
    }

    [TestMethod]
    public async Task ShouldTimeoutAfterSpecifiedDuration()
    {
        // Arrange: Simulate a long-running task to trigger timeout
        Func<CancellationToken, Task<HttpResponseMessage>> action = async (ct) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(15), ct); // Simulate long task
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        };

        // Act and Assert
        await Assert.ThrowsExceptionAsync<TimeoutRejectedException>(() =>
            _resiliencePipelineProvider.ExecuteAsync(action, CancellationToken.None));
    }

    [TestMethod]
    public async Task ShouldHonorRateLimiting()
    {
        // Arrange: Simulate 120 requests in rapid succession, exceeding the rate limit of 10 per second
        int successfulRequests = 0;
        Func<CancellationToken, Task<HttpResponseMessage>> action = (ct) =>
        {
            successfulRequests++;
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        };

        var tasks = new Task<HttpResponseMessage>[1001];
        for (int i = 0; i < 1001; i++)
        {
            tasks[i] = _resiliencePipelineProvider.ExecuteAsync(action, CancellationToken.None);
        }

        // Act: Run all the tasks
        await Task.WhenAll(tasks);

        // Assert: Only 1000 requests should succeed due to rate limiting
        Assert.AreEqual(1001, successfulRequests, "Rate limiter should allow only 1000 requests per second.");
    }

    [TestMethod]
    public async Task ShouldOpenCircuitAfterFailures()
    {
        // Arrange: Simulate 5 consecutive failures to trip the circuit breaker
        Func<CancellationToken, Task<HttpResponseMessage>> action = (ct) =>
        {
            throw new HttpRequestException("Simulated failure");
        };

        // Act: Trigger the circuit breaker by causing multiple failures
        for (int i = 0; i < 5; i++)
        {
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() =>
                _resiliencePipelineProvider.ExecuteAsync(action, CancellationToken.None));
        }

        // Circuit should now be open and immediately reject further attempts
        await Assert.ThrowsExceptionAsync<BrokenCircuitException>(() =>
            _resiliencePipelineProvider.ExecuteAsync(action, CancellationToken.None));
    }
}