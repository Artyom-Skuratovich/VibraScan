namespace VibraScan.WinForms.Views.Abstractions
{
    public interface IView
    {
        event EventHandler ViewLoaded;

        event EventHandler ViewCloased;

        void Show();

        void Close();

        bool? ShowModal();
    }
}