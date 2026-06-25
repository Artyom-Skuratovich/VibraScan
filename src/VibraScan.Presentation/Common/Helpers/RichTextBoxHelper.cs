using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using VibraScan.Application.DataImport.Commands.StartBulkImport;

namespace VibraScan.Presentation.Common.Helpers
{
    public static class RichTextBoxHelper
    {
        public static readonly DependencyProperty ImportResultsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ImportResultsSource",
                typeof(IEnumerable<BulkImportResult>),
                typeof(RichTextBoxHelper),
                new PropertyMetadata(null, OnImportResultsSourceChanged));

        public static IEnumerable<BulkImportResult> GetImportResultsSource(DependencyObject obj) =>
            (IEnumerable<BulkImportResult>)obj.GetValue(ImportResultsSourceProperty);

        public static void SetImportResultsSource(DependencyObject obj, IEnumerable<BulkImportResult> value) =>
            obj.SetValue(ImportResultsSourceProperty, value);

        private static readonly SolidColorBrush GrayBrush = CreateFrozenBrush("#64748B");
        private static readonly SolidColorBrush TextBrush = CreateFrozenBrush("#E2E8F0");
        private static readonly SolidColorBrush GreenBrush = CreateFrozenBrush("#10B981");
        private static readonly SolidColorBrush RedBrush = CreateFrozenBrush("#EF4444");
        private static readonly SolidColorBrush LightRedBrush = CreateFrozenBrush("#FDA4AF");
        private static readonly SolidColorBrush BorderBrush = CreateFrozenBrush("#334155");

        private static SolidColorBrush CreateFrozenBrush(string hex)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            brush.Freeze();
            return brush;
        }

        private static void OnImportResultsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBox rtb) return;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= (s, e) => HandleCollectionChanged(rtb, e);
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                var doc = rtb.Document ?? new FlowDocument();
                doc.PagePadding = new Thickness(0);
                doc.Blocks.Clear();
                rtb.Document = doc;

                if (newCollection is IEnumerable<BulkImportResult> initialList)
                {
                    foreach (var item in initialList)
                    {
                        rtb.Document.Blocks.Add(CreateLogSection(item));
                    }
                }

                newCollection.CollectionChanged += (s, e) => HandleCollectionChanged(rtb, e);
            }
        }

        private static void HandleCollectionChanged(RichTextBox rtb, NotifyCollectionChangedEventArgs e)
        {
            if (rtb.Document == null) return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (BulkImportResult newItem in e.NewItems)
                        {
                            rtb.Document.Blocks.Add(CreateLogSection(newItem));
                        }
                        rtb.ScrollToEnd();
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if ((e.OldItems != null) && (rtb.Document.Blocks.Count > 0))
                    {
                        var startIndex = e.OldStartingIndex;
                        var countToRemove = e.OldItems.Count;

                        for (int i = startIndex + countToRemove - 1; i >= startIndex; i--)
                        {
                            if ((i >= 0) && (i < rtb.Document.Blocks.Count))
                            {
                                var blockToRemove = rtb.Document.Blocks.ElementAt(i);
                                rtb.Document.Blocks.Remove(blockToRemove);
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    rtb.Document.Blocks.Clear();
                    break;
            }
        }

        private static Section CreateLogSection(BulkImportResult result)
        {
            var totalFiles = result.Details.Count;
            var successCount = result.Details.Count(d => d.IsSuccess);
            var failedCount = totalFiles - successCount;

            var section = new Section
            {
                BorderBrush = BorderBrush,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Margin = new Thickness(0, 0, 0, 12),
                Padding = new Thickness(0, 0, 0, 8)
            };

            var headerParagraph = new Paragraph();
            headerParagraph.Inlines.Add(new Run($"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] ") { Foreground = GrayBrush });
            headerParagraph.Inlines.Add(new Run("ИМПОРТ ДАННЫХ: ") { Foreground = TextBrush });

            if (result.AllSucceeded)
            {
                headerParagraph.Inlines.Add(new Run("УСПЕХ") { Foreground = GreenBrush, FontWeight = FontWeights.Bold });
            }
            else
            {
                headerParagraph.Inlines.Add(new Run("ОШИБКА") { Foreground = RedBrush, FontWeight = FontWeights.Bold });
            }
            section.Blocks.Add(headerParagraph);

            var prefix = failedCount > 0 ? "├──" : "└──";
            var statsParagraph = new Paragraph(new Run($"{prefix} Всего файлов: {totalFiles} | Успешно: {successCount} | Ошибок: {failedCount}") { Foreground = TextBrush });
            section.Blocks.Add(statsParagraph);

            if (failedCount > 0)
            {
                var failedParagraph = new Paragraph();
                prefix = failedCount > 3 ? "├──" : "└──";
                failedParagraph.Inlines.Add(new Run($"{prefix} Файлы с ошибкой:\n") { Foreground = GrayBrush });

                var sampleFailed = result.Details.Where(d => !d.IsSuccess).Take(3).ToList();

                for (int i = 0; i < sampleFailed.Count; i++)
                {
                    var file = sampleFailed[i];
                    prefix = (i == sampleFailed.Count - 1 && failedCount <= 3) ? "    └── " : "    ├── ";
                    failedParagraph.Inlines.Add(new Run($"{prefix}{file.SourceName} -> {file.Error?.Message ?? file.Message}\n") { Foreground = LightRedBrush });
                }

                if (failedCount > 3)
                {
                    failedParagraph.Inlines.Add(new Run($"    └── ... и еще ({failedCount - 3}) файлов с ошибками\n") { Foreground = GrayBrush, FontStyle = FontStyles.Italic });
                }
                section.Blocks.Add(failedParagraph);
            }

            return section;
        }
    }
}