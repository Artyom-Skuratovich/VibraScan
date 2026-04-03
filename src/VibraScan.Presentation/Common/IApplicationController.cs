using VibraScan.Presentation.Presenters.Base;

namespace VibraScan.Presentation.Common
{
    public interface IApplicationController
    {
        void Run<TPresenter>() where TPresenter : class, IPresenter;

        void Run<TPresenter, TArg>(TArg argument) where TPresenter : class, IPresenter<TArg>;
    }
}