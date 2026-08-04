namespace VibraScan.Presentation.Services.Interfaces
{
    public interface IFileDialogService
    {
        string? OpenFile(string filter, string initialDirectory = "");

        string[] OpenFiles(string filter, string initialDirectory = "");
    }
}