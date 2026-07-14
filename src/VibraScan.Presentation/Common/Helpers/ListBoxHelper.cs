using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using VibraScan.Presentation.Common.Interfaces;

namespace VibraScan.Presentation.Common.Helpers
{
    public static class ListBoxHelper
    {
        public static readonly DependencyProperty EnableSmartScrollProperty =
            DependencyProperty.RegisterAttached(
                "EnableSmartScroll",
                typeof(bool),
                typeof(ListBoxHelper),
                new PropertyMetadata(false, OnEnableSmartScrollChanged));

        public static bool GetEnableSmartScroll(DependencyObject obj) => (bool)obj.GetValue(EnableSmartScrollProperty);
        public static void SetEnableSmartScroll(DependencyObject obj, bool value) => obj.SetValue(EnableSmartScrollProperty, value);

        public static readonly DependencyProperty ResetScrollTriggerProperty =
            DependencyProperty.RegisterAttached(
                "ResetScrollTrigger",
                typeof(bool),
                typeof(ListBoxHelper),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnResetScrollTriggerChanged));

        public static bool GetResetScrollTrigger(DependencyObject obj) => (bool)obj.GetValue(ResetScrollTriggerProperty);
        public static void SetResetScrollTrigger(DependencyObject obj, bool value) => obj.SetValue(ResetScrollTriggerProperty, value);

        private static readonly DependencyProperty IsAutoScrollActiveProperty =
            DependencyProperty.RegisterAttached(
                "IsAutoScrollActive",
                typeof(bool),
                typeof(ListBoxHelper),
                new PropertyMetadata(true));

        private static void OnEnableSmartScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox && (bool)e.NewValue)
            {
                listBox.Loaded += OnListBoxLoaded;
                listBox.Unloaded += OnListBoxUnloaded;
            }
        }

        private static void OnListBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not ListBox listBox) return;

            listBox.PreviewMouseWheel += OnUserScrollInterrupted;
            listBox.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnUserDragInterrupted));

            if (listBox.ItemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged -= OnItemsSourceCollectionChanged;
                collection.CollectionChanged += OnItemsSourceCollectionChanged;

                SubscribeItems(listBox.Items);
            }
        }

        private static void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                SubscribeItems(e.NewItems);
            }

            if (e.OldItems != null)
            {
                UnsubscribeItems(e.OldItems);
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var activeWindow = GetActiveWindow();
                if (activeWindow == null) return;

                var listBox = FindListBoxWithSmartScroll(activeWindow);
                if (listBox != null)
                {
                    listBox.SetValue(IsAutoScrollActiveProperty, true);
                    SubscribeItems(listBox.Items);
                }
            }
        }

        private static void OnListBoxUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is not ListBox listBox) return;

            listBox.PreviewMouseWheel -= OnUserScrollInterrupted;
            listBox.RemoveHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnUserDragInterrupted));

            if (listBox.ItemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged -= OnItemsSourceCollectionChanged;
                UnsubscribeItems(listBox.Items);
            }
        }

        private static void OnResetScrollTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox || !(bool)e.NewValue) return;

            listBox.SetValue(IsAutoScrollActiveProperty, true);

            if (listBox.Items.Count > 0)
            {
                listBox.ScrollIntoView(listBox.Items[0]);
            }

            listBox.SetCurrentValue(ResetScrollTriggerProperty, false);
        }

        private static void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not ITrackableTask trackableTask || e.PropertyName != nameof(ITrackableTask.IsCompleted)) return;
            if (!trackableTask.IsCompleted) return;

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var activeWindow = GetActiveWindow();
                if (activeWindow == null) return;

                var listBox = FindListBoxWithSmartScroll(activeWindow);

                if ((listBox == null) || !(bool)listBox.GetValue(IsAutoScrollActiveProperty)) return;

                var nextIndex = listBox.Items.IndexOf(trackableTask) + 1;
                if (nextIndex < listBox.Items.Count)
                {
                    var nextItem = listBox.Items[nextIndex];
                    listBox.ScrollIntoView(nextItem);
                }
            }));
        }

        private static void OnUserScrollInterrupted(object sender, MouseWheelEventArgs e)
        {
            DisableAutoScroll(sender);
        }

        private static void OnUserDragInterrupted(object sender, DragStartedEventArgs e)
        {
            DisableAutoScroll(sender);
        }

        private static void DisableAutoScroll(object sender)
        {
            if (sender is DependencyObject visualObject)
            {
                var listBox = visualObject as ListBox ?? FindVisualParent<ListBox>(visualObject);
                listBox?.SetValue(IsAutoScrollActiveProperty, false);
            }
        }

        private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;
            return FindVisualParent<T>(parentObject);
        }

        private static ListBox? FindListBoxWithSmartScroll(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is ListBox lb && GetEnableSmartScroll(lb)) return lb;

                var result = FindListBoxWithSmartScroll(child);
                if (result != null) return result;
            }

            return null;
        }

        private static Window? GetActiveWindow()
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.IsActive) return window;
            }


            if (Keyboard.FocusedElement is DependencyObject focusedElement)
            {
                return Window.GetWindow(focusedElement);
            }

            return System.Windows.Application.Current.MainWindow;
        }

        private static void SubscribeItems(IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is ITrackableTask trackableTask)
                {
                    trackableTask.PropertyChanged -= OnItemPropertyChanged;
                    trackableTask.PropertyChanged += OnItemPropertyChanged;
                }
            }
        }

        private static void UnsubscribeItems(IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is ITrackableTask trackableTask)
                {
                    trackableTask.PropertyChanged -= OnItemPropertyChanged;
                }
            }
        }
    }
}