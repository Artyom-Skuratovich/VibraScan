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
                if (!textBox.IsLoaded)
                {
                    textBox.Loaded += (s, _) =>
                    {
                        var tb = (TextBox)s;
                        tb.Tag = tb.Padding;
                        UpdateAdornerVisibility(tb);
                    };
                }
                else
                {
                    UpdateTextBoxMinWidth(textBox);
                }

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

                if (string.IsNullOrEmpty(textBox.Text) && (textBox.HorizontalContentAlignment == HorizontalAlignment.Center))
                {
                    var offset = CalculatePlaceholderOffset(textBox);
                    var basePadding = (Thickness)textBox.Tag;
                    textBox.Padding = new Thickness(offset, basePadding.Top, basePadding.Right, basePadding.Bottom);
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                }
                else if (string.IsNullOrEmpty(textBox.Text))
                {
                    if (textBox.Tag is Thickness basePadding)
                    {
                        textBox.Padding = basePadding;
                    }
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                }
            }
        }

        private static void UpdateTextBoxMinWidth(TextBox textBox)
        {
            var placeholder = GetPlaceholder(textBox);

            if (string.IsNullOrEmpty(placeholder)) return;

            var typeface = new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch);
            var formattedText = new FormattedText(
                    placeholder,
                    CultureInfo.CurrentCulture,
                    textBox.FlowDirection,
                    typeface,
                    textBox.FontSize,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(textBox).PixelsPerDip);

            var requiredWidth = formattedText.Width + textBox.Padding.Left + textBox.Padding.Right + textBox.BorderThickness.Left + textBox.BorderThickness.Right + 4;

            textBox.MinWidth = requiredWidth;
        }

        private static double CalculatePlaceholderOffset(TextBox textBox)
        {
            var placeholder = GetPlaceholder(textBox);

            if (string.IsNullOrEmpty(placeholder)) return textBox.Padding.Left;

            var typeface = new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch);
            var formattedText = new FormattedText(
                    placeholder,
                    CultureInfo.CurrentCulture,
                    textBox.FlowDirection,
                    typeface,
                    textBox.FontSize,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(textBox).PixelsPerDip);

            var contentWidth = textBox.ActualWidth - textBox.BorderThickness.Left - textBox.BorderThickness.Right + 2;
            var offset = (contentWidth - formattedText.Width) / 2;

            return Math.Max(0, offset);
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

                var typeface = new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch);
                var foreground = textBox.TryFindResource("DisabledElementTextBrush") as Brush ?? SystemColors.InactiveCaptionBrush;

                var formattedText = new FormattedText(
                    placeholderValue,
                    CultureInfo.CurrentCulture,
                    textBox.FlowDirection,
                    typeface,
                    textBox.FontSize,
                    foreground,
                    VisualTreeHelper.GetDpi(textBox).PixelsPerDip)
                {
                    MaxTextWidth = Math.Max(1, textBox.ActualWidth - textBox.Padding.Left - textBox.Padding.Right - 4),
                };

                //var availableWidth = textBox.ActualWidth - textBox.Padding.Left - textBox.Padding.Right - textBox.BorderThickness.Left - textBox.BorderThickness.Right - 4;
                var availableHeight = textBox.ActualHeight - textBox.Padding.Top - textBox.Padding.Bottom - textBox.BorderThickness.Top - textBox.BorderThickness.Bottom;

                //var left = textBox.HorizontalContentAlignment switch
                //{
                //    HorizontalAlignment.Center => textBox.BorderThickness.Left + textBox.Padding.Left + 2 + (availableWidth - formattedText.Width) / 2,
                //    HorizontalAlignment.Right => textBox.ActualWidth - textBox.BorderThickness.Right - textBox.Padding.Right - 2 - formattedText.Width,
                //    _ => textBox.BorderThickness.Left + textBox.Padding.Left + 2
                //};
                var top = textBox.VerticalContentAlignment switch
                {
                    VerticalAlignment.Top => textBox.BorderThickness.Top + textBox.Padding.Top,
                    VerticalAlignment.Bottom => textBox.ActualHeight - textBox.BorderThickness.Bottom - textBox.Padding.Bottom - formattedText.Height,
                    _ => textBox.BorderThickness.Top + textBox.Padding.Top + (availableHeight - formattedText.Height) / 2,
                };

                drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, textBox.ActualWidth, textBox.ActualHeight)));
                drawingContext.DrawText(formattedText, new Point(0, top));
                drawingContext.Pop();
            }
        }
    }
}