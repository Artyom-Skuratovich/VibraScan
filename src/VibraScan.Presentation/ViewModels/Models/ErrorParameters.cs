namespace VibraScan.Presentation.ViewModels.Models
{
    public record ErrorParameters(
        string Title,
        string Message,
        Exception? Exception = null);
}