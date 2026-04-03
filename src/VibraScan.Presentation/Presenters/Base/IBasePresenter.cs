using VibraScan.Presentation.Views.Base;

namespace VibraScan.Presentation.Presenters.Base
{
    public interface IBasePresenter
    {
        IView View { get; }
    }
}