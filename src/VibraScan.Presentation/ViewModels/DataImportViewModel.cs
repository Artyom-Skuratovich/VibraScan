using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.DataImport;
using VibraScan.Application.DataImport.Commands.StartBulkImport;
using VibraScan.Presentation.ViewModels.Components;

namespace VibraScan.Presentation.ViewModels
{
    public partial class DataImportViewModel : ObservableObject
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public DataImportViewModel(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            Files.CollectionChanged += (s, e) => StartImportCommand.NotifyCanExecuteChanged();
        }

        public ObservableCollection<FileItemViewModel> Files { get; } = [];

        public bool CanStartImport => Files.Count > 0;

        [RelayCommand(CanExecute = nameof(CanStartImport))]
        private async Task StartImportAsync(CancellationToken ct)
        {
            // TODO: получить источники (стримы, названия файлов).

            int currentFileIndex = 0;

            var overallProgress = new Progress<BulkImportProgress>(progress =>
            {
                currentFileIndex = progress.CurrentIndex - 1;
                // TODO: привязать к общему прогрессу (сообщение и номер обработанного файла).
            });

            var segmentProgress = new Progress<ImportProgress>(progress =>
            {
                if ((currentFileIndex >= 0) && (currentFileIndex < Files.Count))
                {
                    var file = Files[currentFileIndex];
                    file.Stage = progress.CurrentStage;
                    file.ImportPercentage = progress.Percentage;
                }
            });

            var command = new StartBulkImportCommand
            {
                Sources = [], // TODO: приявязать реальные источники.
                OverallProgress = overallProgress,
                SegmentProgress = segmentProgress
            };

            // TODO: подумать о результате.
            _ = await _commandDispatcher.SendAsync(command, ct);
        }
    }
}