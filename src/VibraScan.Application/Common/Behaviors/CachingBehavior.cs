using MediatR;
using Microsoft.Extensions.Caching.Memory;
using VibraScan.Application.Common.Interfaces;

namespace VibraScan.Application.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse>(IMemoryCache cache) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICachableQuery
    {
        private readonly IMemoryCache _cache = cache;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (request is not ICachableQuery cachableQuery)
            {
                return await next(ct);
            }

            if (_cache.TryGetValue(cachableQuery.ChacheKey, out TResponse? cachedResponse))
            {
                return cachedResponse!;
            }

            var response = await next(ct);

            var cacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = cachableQuery.ExpirationTime,
                Size = 1
            };

            _cache.Set(cachableQuery.ChacheKey, response, cacheOptions);

            return response;
        }
    }
}