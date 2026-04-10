using CommunityToolkit.Mvvm.ComponentModel;
using VibraScan.Application.Common.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public partial class ChartsViewModel(ICommandDispatcher commandDispatcher) : ObservableObject
    {
        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
    }
}