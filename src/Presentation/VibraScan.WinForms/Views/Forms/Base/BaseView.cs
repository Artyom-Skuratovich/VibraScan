using System.Text;
using VibraScan.WinForms.Views.Abstractions;

namespace VibraScan.WinForms.Views.Forms.Base
{
    public partial class BaseView : Form, IView
    {
        public BaseView()
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
                    var details = new StringBuilder();

                    details.AppendLine($"Тип: {ex.GetType().Name}");
                    details.AppendLine($"Сообщение: {ex.Message}");

                    if (ex.InnerException != null)
                    {
                        details.AppendLine($"Внутренняя ошибка: {ex.InnerException.Message}");
                    }

                    details.AppendLine("\n[StackTrace]");
                    details.AppendLine(ex.StackTrace);

                    page.Expander = new TaskDialogExpander()
                    {
                        Text = details.ToString(),
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