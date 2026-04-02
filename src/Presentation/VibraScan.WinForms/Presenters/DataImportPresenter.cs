using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.DataImport;
using VibraScan.Application.DataImport.Commands.StartBulkImport;
using VibraScan.Application.DataImport.Commands.StartImport;
using VibraScan.WinForms.Presenters.Abstractions;
using VibraScan.WinForms.Views.Abstractions;

namespace VibraScan.WinForms.Presenters
{
    public class DataImportPresenter : IPresenter
    {
        private readonly IDataImportView _view;
        private readonly ICommandDispatcher _dispatcher;
        private readonly string[] _files;
        private CancellationTokenSource? _cts;

        public DataImportPresenter(IDataImportView view, ICommandDispatcher dispatcher, string[] files)
        {
            _view = view;
            _dispatcher = dispatcher;
            _files = files;

            _view.ViewLoaded += OnViewLoaded;
            _view.ViewClosed += OnViewClosed;
            _view.CancelRequested += OnCancelRequested;
        }

        public void Run()
        {
            _view.ShowModal();
        }

        private async void OnViewLoaded(object? sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            var sources = new List<ImportSource>(_files.Length);

            try
            {
                foreach (var file in _files)
                {
                    sources.Add(new ImportSource(Path.GetFileName(file), File.OpenRead(file)));
                }

                _view.PrepareForImport(_files.Length);
                _view.SetCancelable(true);

                var segmentProgress = new Progress<ImportProgress>(p => _view.UpdateFileProgress(p.Percentage, p.CurrentStage));

                if (_files.Length > 1)
                {
                    var overallProgress = new Progress<BulkImportProgress>(p => _view.UpdateOverallProgress(p.CurrentIndex, p.Description));

                    var command = new StartBulkImportCommand
                    {
                        Sources = sources,
                        SegmentProgress = segmentProgress,
                        OverallProgress = overallProgress
                    };

                    _view.ShowResults(await _dispatcher.SendAsync(command, _cts.Token));
                }
                else
                {
                    var command = new StartImportCommand
                    {
                        Source = sources[0],
                        Progress = segmentProgress
                    };

                    _view.ShowResults(await _dispatcher.SendAsync(command, _cts.Token));
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка импорта", "Непредвиденная ошибка", ex);
            }
            finally
            {
                _view.SetCancelable(false);

                foreach (var src in sources)
                {
                    src.Data?.Dispose();
                }
                sources.Clear();

                _cts?.Dispose();
                _cts = null;
            }
        }

        private void OnViewClosed(object? sender, EventArgs e)
        {
            _cts?.Cancel();

            _view.CancelRequested -= OnCancelRequested;
            _view.ViewClosed -= OnViewClosed;
            _view.ViewLoaded -= OnViewLoaded;
        }

        private void OnCancelRequested(object? sender, EventArgs e)
        {
            _cts?.Cancel();
            _view.SetCancelable(false);
        }
    }
}