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
            _view.CancelRequested += OnCancelRequested;
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        private async void OnViewLoaded(object? sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            var openedStreams = new List<Stream>(_files.Length);

            try
            {
                foreach (var file in _files)
                {
                    openedStreams.Add(File.OpenRead(file));
                }
                var isBulk = _files.Length > 1;

                var segmentProgress = new Progress<ImportProgress>(p =>
                {
                    // TODO: настройка _view.
                });

                if (isBulk)
                {
                    var overallProgress = new Progress<BulkImportProgress>(p =>
                    {
                        // TODO: настройка _view.
                    });

                    var command = new StartBulkImportCommand
                    {
                        DataStreams = openedStreams,
                        SegmentProgress = segmentProgress,
                        OverallProgress = overallProgress
                    };

                    var result = await _dispatcher.SendAsync(command, _cts.Token);
                    // TODO: обработка результата.
                }
                else
                {
                    var command = new StartImportCommand
                    {
                        DataStream = openedStreams[0],
                        Progress = segmentProgress
                    };

                    var result = await _dispatcher.SendAsync(command, _cts.Token);
                    // TODO: обработка результата.
                }
            }
            catch
            {
                // TODO: обработка ошибки.
            }
            finally
            {
                openedStreams.ForEach(s => s?.Dispose());
                openedStreams.Clear();

                _cts?.Dispose();
                _cts = null;
            }
        }

        private void OnCancelRequested(object? sender, EventArgs e)
        {
            _cts?.Cancel();
        }
    }
}