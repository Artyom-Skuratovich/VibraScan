namespace VibraScan.Presentation.Models
{
    public record ErrorParameters(
        string Title,
        string Message,
        Exception? Exception = null);
}