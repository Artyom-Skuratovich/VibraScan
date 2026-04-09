namespace VibraScan.Presentation.Services.Interfaces
{
    public interface IWindowService
    {
        void Show<TVm>() where TVm : class;

        void Show<TVm, TParam>(TParam param) where TVm : class;

        bool? ShowDialog<TVm>() where TVm : class;

        bool? ShowDialog<TVm, TParam>(TParam param) where TVm : class;

        void Close(object model);
    }
}