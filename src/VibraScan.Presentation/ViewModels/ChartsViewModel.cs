using CommunityToolkit.Mvvm.ComponentModel;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public partial class ChartsViewModel(ICommandDispatcher commandDispatcher, IErrorVisualizerService errorVisualizerService) : ObservableObject, ISupportCancellation
    {
        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
        private readonly IErrorVisualizerService _errorVisualizerService = errorVisualizerService;

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}