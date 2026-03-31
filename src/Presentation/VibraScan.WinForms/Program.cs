using VibraScan.WinForms.Views.Forms;

namespace VibraScan.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            System.Windows.Forms.Application.Run(new DataImportForm());
        }
    }
}