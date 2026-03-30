using MediatR;

namespace VibraScan.Application.Common.Interfaces
{
    /// <summary>
    /// Обеспечивает единую точку входа для выполнения комманд и запросов в изолированном контексте.
    /// </summary>
    /// <remarks>
    /// Гарантирует полную изоляцию ресурсов для каждого вызова,
    /// управляя жизненным циклом зависимостей, необходимых для обработки запроса.
    /// </remarks>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Выполняет запрос и возвращает результат в рамках атомарной операции.
        /// </summary>
        /// <typeparam name="TResponse">Тип возвращаемых данных.</typeparam>
        /// <param name="request">Объект запроса, содержащий входные данные.</param>
        /// <param name="ct">Токен отмены для прерывания операции.</param>
        /// <returns>Результат выполнения запроса.</returns>
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);

        Task ExecuteInScopeAsync(Func<IMediator, Task> action, CancellationToken ct = default);

        Task<TResponse> ExecuteInScopeAsync<TResponse>(Func<IMediator, Task<TResponse>> action, CancellationToken ct = default);
    }
}