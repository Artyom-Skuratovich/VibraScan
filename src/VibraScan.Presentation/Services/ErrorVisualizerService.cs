using VibraScan.Presentation.Models;
using VibraScan.Presentation.Services.Interfaces;
using VibraScan.Presentation.ViewModels;

namespace VibraScan.Presentation.Services
{
    public class ErrorVisualizerService(IWindowService windowService) : IErrorVisualizerService
    {
        private readonly IWindowService _windowService = windowService;

        public void ShowError(string title, string message, Exception? ex = null)
        {
            var args = new ErrorParameters(title, message, ex);
            _windowService.ShowDialog<ErrorViewModel, ErrorParameters>(args);
        }
    }
}