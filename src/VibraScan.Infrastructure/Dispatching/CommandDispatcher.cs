using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VibraScan.Application.Common.Interfaces;

namespace VibraScan.Infrastructure.Dispatching
{
    internal class CommandDispatcher(IServiceScopeFactory scopeFactory) : ICommandDispatcher
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task ExecuteInScopeAsync(Func<IMediator, Task> action, CancellationToken ct = default)
        {
            using var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await action(mediator);
        }

        public async Task<TResponse> ExecuteInScopeAsync<TResponse>(Func<IMediator, Task<TResponse>> action, CancellationToken ct = default)
        {
            using var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            return await action(mediator);
        }

        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
        {
            using var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            return await mediator.Send(request, ct);
        }
    }
}