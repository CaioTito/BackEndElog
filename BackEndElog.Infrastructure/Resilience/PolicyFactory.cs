using BackEndElog.Shared.Results;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace BackEndElog.Infrastructure.Resilience
{
    public static class PolicyFactory
    {
        public static AsyncRetryPolicy<Result<T?>> CreateRetryPolicy<T>(ILogger logger, int retryCount = 3)
        {
            return Policy
                .HandleResult<Result<T?>>(r => !r.IsSuccess && r.Error?.Code == 429)
                .WaitAndRetryAsync(
                    retryCount: retryCount,
                    sleepDurationProvider: (retryAttempt, result, context) =>
                    {
                        if (context.TryGetValue("RetryAfter", out var retryAfterObj) && retryAfterObj is TimeSpan retryAfter)
                            return retryAfter;

                        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    },
                    onRetryAsync: async (result, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning(
                            "Tentativa {RetryAttempt} falhou com erro: {ErrorDescription}. Retentando em {RetryAfterSeconds} segundos...",
                            retryAttempt,
                            result.Result?.Error?.Description,
                            timespan.TotalSeconds
                        );

                        await Task.CompletedTask;
                    });
        }
    }
}
