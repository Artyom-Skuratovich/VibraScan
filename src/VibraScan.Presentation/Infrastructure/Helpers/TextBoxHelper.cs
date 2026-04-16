using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace VibraScan.Presentation.Infrastructure.Helpers
{
    public static class TextBoxHelper
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(string),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(
                    defaultValue: null,
                    propertyChangedCallback: OnPlaceholderChanged)
                );

        public static string GetPlaceholder(DependencyObject obj) => (string)obj.GetValue(PlaceholderProperty);

        public static void SetPlaceholder(DependencyObject obj, string value) => obj.SetValue(PlaceholderProperty, value);

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.TextChanged -= TextBoxControlTextChanged;
                textBox.TextChanged += TextBoxControlTextChanged;

                textBox.IsVisibleChanged -= TextBoxIsVisibleChanged;
                textBox.IsVisibleChanged += TextBoxIsVisibleChanged;

                if (GetOrCreateAdorner(textBox, out var adorner))
                {
                    adorner!.InvalidateVisual();
                }
            }
        }

        private static void TextBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox textBox && (bool)e.NewValue)
            {
                UpdateAdornerVisibility(textBox);
            }
        }

        private static void TextBoxControlTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                UpdateAdornerVisibility(textBox);
            }
        }

        private static bool GetOrCreateAdorner(TextBox textBox, out PlaceholderAdorner? adorner)
        {
            var layer = AdornerLayer.GetAdornerLayer(textBox);

            if (layer == null)
            {
                adorner = null;
                return false;
            }

            adorner = layer.GetAdorners(textBox)?.OfType<PlaceholderAdorner>().FirstOrDefault();

            if (adorner == null)
            {
                adorner = new PlaceholderAdorner(textBox);
                layer.Add(adorner);
            }

            return true;
        }

        private static void UpdateAdornerVisibility(TextBox textBox)
        {
            if (GetOrCreateAdorner(textBox, out var adorner))
            {
                adorner!.Visibility = textBox.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private class PlaceholderAdorner : Adorner
        {
            public PlaceholderAdorner(TextBox textBox)
                : base(textBox)
            {
                IsHitTestVisible = false;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                var textBox = (TextBox)AdornedElement;
                var placeholderValue = GetPlaceholder(textBox);

                if (string.IsNullOrEmpty(placeholderValue)) return;

                var text = new FormattedText(
                    placeholderValue,
                    CultureInfo.CurrentCulture,
                    textBox.FlowDirection,
                    new Typeface(textBox.FontFamily,
                                 textBox.FontStyle,
                                 textBox.FontWeight,
                                 textBox.FontStretch),
                    textBox.FontSize,
                    SystemColors.InactiveCaptionBrush,
                    VisualTreeHelper.GetDpi(textBox).PixelsPerDip);

                var rect = textBox.GetRectFromCharacterIndex(0, true);

                if (rect != Rect.Empty)
                {
                    drawingContext.DrawText(text, new Point(rect.Left, rect.Top));
                }
                else
                {
                    drawingContext.DrawText(text, new Point(textBox.Padding.Left + 3, textBox.Padding.Top));
                }
            }
        }
    }
}