namespace VibraScan.Presentation.Views.Base
{
    public interface IView
    {
        event Action Load;
        event Action Closed;

        bool IsDisposed { get; }

        void Show();

        bool? ShowDialog();

        void Close();
    }
}