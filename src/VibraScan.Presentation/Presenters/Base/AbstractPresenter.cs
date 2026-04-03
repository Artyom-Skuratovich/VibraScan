using VibraScan.Presentation.Common;
using VibraScan.Presentation.Views.Base;

namespace VibraScan.Presentation.Presenters.Base
{
    public abstract class AbstractPresenter<TView>(IView view, IApplicationController controller) : IBasePresenter, IDisposable where TView : IView
    {
        private CancellationTokenSource? _cts;
        private bool _disposed;

        public IView View { get; } = view;

        protected IApplicationController Controller { get; } = controller;

        protected CancellationToken CancellationToken => (_cts ??= new CancellationTokenSource()).Token;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cts?.Cancel();
                    _cts?.Dispose();
                    _cts = null;
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}