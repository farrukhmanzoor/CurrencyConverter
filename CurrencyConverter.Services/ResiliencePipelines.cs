using Polly;
using Polly.CircuitBreaker;
using Polly.RateLimiting;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace CurrencyConverter.Services
{
    public class ResiliencePipelineProvider
    {
        private readonly ResiliencePipeline<HttpResponseMessage> _resiliencePipeline;

        public ResiliencePipelineProvider()
        {
            // Initialize the resilience pipeline with rate limiter, retry, timeout, and circuit breaker
            this._resiliencePipeline = GetPipeBuilder();
        }

        private ResiliencePipeline<HttpResponseMessage> GetPipeBuilder()
        {


            var builder = new ResiliencePipelineBuilder<HttpResponseMessage>()


               // Add retry: Retry 3 times on failure with exponential backoff
               .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
               {
                   // PredicateBuilder is a convenience API that can used to configure the ShouldHandle predicate.
                   ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                   .Handle<HttpRequestException>()
                   .Handle<RateLimiterRejectedException>()
                   .HandleResult(static result => result.StatusCode == HttpStatusCode.InternalServerError),
                   MaxRetryAttempts = 3,
               })
               //.AddConcurrencyLimiter(100, 50)
                .AddRateLimiter(new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions()
                {
                    SegmentsPerWindow = 100,
                    PermitLimit = 1000,
                    Window = TimeSpan.FromSeconds(1),
                }))

                // Add timeout: Timeout after 10 seconds
                .AddTimeout(TimeSpan.FromSeconds(10))

            .Build();
            return builder;

        }

        public async Task<HttpResponseMessage> ExecuteAsync(Func<CancellationToken, Task<HttpResponseMessage>> action, CancellationToken cancellationToken)
        {
            // Executes the pipeline with the provided action (HTTP request in this case)
            return await _resiliencePipeline.ExecuteAsync<HttpResponseMessage>(async ct => await action(ct), cancellationToken); ;
        }

    }
}
