using Microsoft.Win32;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string? OpenFile(string filter, string initialDirectory = "")
        {
            var dialog = CreateDialog(filter, initialDirectory, false);
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string[] OpenFiles(string filter, string initialDirectory = "")
        {
            var dialog = CreateDialog(filter, initialDirectory, true);
            return dialog.ShowDialog() == true ? dialog.FileNames : [];
        }

        private static OpenFileDialog CreateDialog(string filter, string initialDirectory, bool multiselect)
        {
            return new OpenFileDialog
            {
                Filter = filter,
                InitialDirectory = initialDirectory,
                Multiselect = multiselect
            };
        }
    }
}