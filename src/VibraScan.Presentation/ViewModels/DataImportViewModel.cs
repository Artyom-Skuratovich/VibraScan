using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.DataImport;
using VibraScan.Application.DataImport.Commands.StartBulkImport;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Services.Interfaces;
using VibraScan.Presentation.ViewModels.Components;

namespace VibraScan.Presentation.ViewModels
{
    public partial class DataImportViewModel : ObservableObject, ICancellable
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IFileDialogService _fileDialog;
        private readonly IErrorVisualizerService _errorVisualizer;

        public DataImportViewModel(ICommandDispatcher commandDispatcher, IFileDialogService fileDialog, IErrorVisualizerService errorVisualizer)
        {
            _commandDispatcher = commandDispatcher;
            _fileDialog = fileDialog;
            _errorVisualizer = errorVisualizer;

            Files.CollectionChanged += (s, e) => StartImportCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty] private string _overallProgressText = string.Empty;
        [ObservableProperty] private int _currentFileNumber;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SelectFilesCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveFileCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelImportCommand))]
        private bool _isImporting;

        public ObservableCollection<FileItemViewModel> Files { get; } = [];

        public bool CanStartImport => Files.Count > 0;

        public void Cancel()
        {
            StartImportCommand.Cancel();
        }

        private bool CanExecuteWhenNotImporting() => !IsImporting;

        [RelayCommand(CanExecute = nameof(CanExecuteWhenNotImporting))]
        private void SelectFiles()
        {
            var filter = "XML файлы (*.xml)|*.xml";
            var files = _fileDialog.OpenFiles(filter);

            if (files.Length > 0)
            {
                Files.Clear();

                foreach (var file in files)
                {
                    Files.Add(new FileItemViewModel
                    {
                        Name = Path.GetFileName(file),
                        FullPath = file
                    });
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteWhenNotImporting))]
        private void RemoveFile(FileItemViewModel? file)
        {
            if ((file != null) && Files.Contains(file))
            {
                Files.Remove(file);
            }
        }

        [RelayCommand(CanExecute = nameof(CanStartImport))]
        private async Task StartImportAsync(CancellationToken ct)
        {
            int currentFileIndex = 0;

            var overallProgress = new Progress<BulkImportProgress>(progress =>
            {
                currentFileIndex = progress.CurrentNumber - 1;

                OverallProgressText = progress.Description;
                CurrentFileNumber = progress.CurrentNumber;

                if ((progress.Error != null) && (currentFileIndex >= 0) && (currentFileIndex < Files.Count))
                {
                    var file = Files[currentFileIndex];
                    file.Error = progress.Error;
                    file.Stage = $"Ошибка: {progress.Error.Message}";
                }
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

            var sources = new List<ImportSource>(Files.Count);

            try
            {
                foreach (var file in Files)
                {
                    sources.Add(new ImportSource(file.Name, new FileStream(file.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)));
                }

                var command = new StartBulkImportCommand
                {
                    Sources = sources,
                    OverallProgress = overallProgress,
                    SegmentProgress = segmentProgress
                };

                IsImporting = true;

                _ = await _commandDispatcher.SendAsync(command, ct);
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError("Критическая ошибка", "Ошибка при импорте данных", ex);
            }
            finally
            {
                IsImporting = false;

                if (ct.IsCancellationRequested)
                {
                    foreach (var file in Files)
                    {
                        file.ResetToDefault();
                    }
                }

                foreach (var source in sources)
                {
                    try
                    {
                        source.Data.Close();
                    }
                    catch { }
                }
            }
        }

        [RelayCommand(CanExecute = nameof(IsImporting))]
        private void CancelImport()
        {
            StartImportCommand.Cancel();
        }
    }
}