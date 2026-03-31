namespace VibraScan.WinForms.Views.Abstractions
{
    public interface IView
    {
        event EventHandler? ViewLoaded;

        event EventHandler? ViewClosed;

        void Show();

        void Close();

        bool? ShowModal();

        void ShowError(string message, string title, Exception? ex = null);
    }
}