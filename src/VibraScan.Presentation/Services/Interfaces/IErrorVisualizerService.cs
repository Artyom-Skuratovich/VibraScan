namespace VibraScan.Presentation.Services.Interfaces
{
    public interface IErrorVisualizerService
    {
        void ShowError(string title, string message, Exception? ex = null);
    }
}