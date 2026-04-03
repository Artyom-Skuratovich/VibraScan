namespace VibraScan.Presentation.Presenters.Base
{
    public interface IPresenter : IBasePresenter
    {
        void Run();
    }

    public interface IPresenter<in TArg> : IBasePresenter
    {
        void Run(TArg argument);
    }
}