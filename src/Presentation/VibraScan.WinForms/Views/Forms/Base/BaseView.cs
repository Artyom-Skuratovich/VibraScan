using System.Text;
using VibraScan.WinForms.Views.Abstractions;

namespace VibraScan.WinForms.Views.Forms.Base
{
    public partial class BaseView : Form, IView
    {
        protected BaseView()
        {
            InitializeComponent();
        }

        public event EventHandler? ViewLoaded;
        public event EventHandler? ViewClosed;

        public virtual void ShowError(string message, string title, Exception? ex = null)
        {
            InvokeIfNeeded(() =>
            {
                var page = new TaskDialogPage()
                {
                    Caption = title,
                    Heading = message,
                    Icon = TaskDialogIcon.Error,
                    Buttons = { TaskDialogButton.OK }
                };

                if (ex != null)
                {
                    var errorDetails = new StringBuilder();
                    var currentEx = ex;
                    int depth = 0;

                    while (currentEx != null)
                    {
                        var prefix = depth == 0 ? "Ошибка" : $"Внутренняя ({depth})";
                        errorDetails.AppendLine($"[{prefix}]: {currentEx.GetType().Name}");
                        errorDetails.AppendLine(currentEx.Message);
                        errorDetails.AppendLine();

                        currentEx = currentEx.InnerException;
                        depth++;
                    }

                    if (!string.IsNullOrEmpty(ex.StackTrace))
                    {
                        errorDetails.AppendLine("[StackTrace]");
                        errorDetails.AppendLine(ex.StackTrace);
                    }

                    page.Expander = new TaskDialogExpander()
                    {
                        Text = errorDetails.ToString(),
                        CollapsedButtonText = "Показать подробности",
                        ExpandedButtonText = "Скрыть подробности"
                    };
                }

                TaskDialog.ShowDialog(this, page);
            });
        }

        public virtual bool? ShowModal()
        {
            var result = ShowDialog();

            if (result == DialogResult.OK) return true;
            if (result == DialogResult.Cancel) return false;

            return null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ViewLoaded?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            ViewClosed?.Invoke(this, EventArgs.Empty);
        }

        protected void InvokeIfNeeded(Action action)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}