namespace VibraScan.Presentation.ViewModels.Components
{
    public record ErrorParameters(
        string Title,
        string Message,
        Exception? Exception = null);
}