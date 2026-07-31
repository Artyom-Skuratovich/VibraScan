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
    public partial class DataImportViewModel(ICommandDispatcher commandDispatcher, IFileDialogService fileDialog, IErrorVisualizerService errorVisualizer) 
        : ObservableObject, ICancellable
    {
        private const int MaxLogCapacity = 10;

        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
        private readonly IFileDialogService _fileDialog = fileDialog;
        private readonly IErrorVisualizerService _errorVisualizer = errorVisualizer;

        [ObservableProperty] private string _overallProgressText = string.Empty;
        [ObservableProperty] private int _currentFileNumber;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SelectFilesCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveFileCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelImportCommand))]
        private bool _isImporting;

        public ObservableCollection<FileItemViewModel> Files { get; } = [];

        public ObservableCollection<BulkImportResult> ImportHistory { get; } = [];

        public bool CanStartImport => Files.Count > 0 && !IsImporting;

        public void Cancel()
        {
            StartImportCommand.Cancel();
        }

        private bool CanExecuteWhenNotImporting() => !IsImporting;

        private void ResetAllFiles(bool clearErrors = true)
        {
            foreach (var file in Files)
            {
                file.ResetToDefault(clearErrors);
            }

            OverallProgressText = string.Empty;
            CurrentFileNumber = 0;
        }

        private void SetImportState(bool isImporting)
        {
            IsImporting = isImporting;
            StartImportCommand.NotifyCanExecuteChanged();
        }

        private Progress<BulkImportProgress> CreateOverallProgress()
        {
            return new Progress<BulkImportProgress>(progress =>
            {
                OverallProgressText = progress.Description;
                CurrentFileNumber = progress.CurrentNumber;
                var fileIndex = progress.CurrentNumber - 1;

                if ((progress.Error != null) && (fileIndex >= 0) && (fileIndex < Files.Count))
                {
                    var file = Files[fileIndex];
                    file.Error = progress.Error;
                    file.Stage = $"Ошибка: {progress.Error.Message}";
                    file.ImportPercentage = 0;
                }
            });
        }

        private Progress<ImportProgress> CreateSegmentProgress()
        {
            return new Progress<ImportProgress>(progress =>
            {
                var fileIndex = CurrentFileNumber - 1;

                if ((fileIndex >= 0) && (fileIndex < Files.Count))
                {
                    var file = Files[fileIndex];
                    file.Stage = progress.CurrentStage;
                    file.ImportPercentage = progress.Percentage;
                }
            });
        }

        private async Task<List<ImportSource>> OpenImportSourceAsync(CancellationToken ct)
        {
            return await Task.Run(() =>
            {
                var resultList = new List<ImportSource>();

                try
                {
                    foreach (var file in Files)
                    {
                        ct.ThrowIfCancellationRequested();
                        var stream = new FileStream(file.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                        resultList.Add(new ImportSource(file.Name, stream));
                    }
                    return resultList;
                }
                catch
                {
                    foreach (var source in resultList)
                    {
                        source.Data?.Dispose();
                    }
                    throw;
                }
            }, ct);
        }

        private void OnImportFinished(BulkImportResult? newResult)
        {
            if (newResult == null) return;

            while (ImportHistory.Count >= MaxLogCapacity)
            {
                ImportHistory.RemoveAt(0);
            }
            ImportHistory.Add(newResult);
        }

        [RelayCommand(CanExecute = nameof(CanExecuteWhenNotImporting))]
        private void SelectFiles()
        {
            var filter = "XML файлы (*.xml)|*.xml";
            var files = _fileDialog.OpenFiles(filter);

            if (files.Length == 0) return;

            Files.Clear();

            foreach (var file in files)
            {
                Files.Add(new FileItemViewModel
                {
                    Name = Path.GetFileName(file),
                    FullPath = file
                });
            }

            StartImportCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanExecuteWhenNotImporting))]
        private void RemoveFile(FileItemViewModel? file)
        {
            if ((file != null) && Files.Contains(file))
            {
                Files.Remove(file);
                StartImportCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand(CanExecute = nameof(CanStartImport))]
        private async Task StartImportAsync(CancellationToken ct)
        {
            ResetAllFiles();

            var overallProgress = CreateOverallProgress();
            var segmentProgress = CreateSegmentProgress();
            var sources = new List<ImportSource>(Files.Count);
            BulkImportResult? result = null;

            try
            {
                SetImportState(true);
                OverallProgressText = "Подготовка файлов к импорту...";
                sources.AddRange(await OpenImportSourceAsync(ct));

                var command = new StartBulkImportCommand
                {
                    Sources = sources,
                    OverallProgress = overallProgress,
                    SegmentProgress = segmentProgress
                };
                result = await _commandDispatcher.SendAsync(command, ct);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError("Критическая ошибка", "Ошибка при импорте данных", ex);
                ResetAllFiles(false);
            }
            finally
            {
                SetImportState(false);

                foreach (var source in sources)
                {
                    await source.Data.DisposeAsync();
                }

                if (ct.IsCancellationRequested)
                {
                    ResetAllFiles();
                }
                OnImportFinished(result);
            }
        }

        [RelayCommand(CanExecute = nameof(IsImporting))]
        private void CancelImport()
        {
            StartImportCommand.Cancel();
        }

        [RelayCommand]
        private void ClearLog()
        {
            ImportHistory.Clear();
        }
    }
}