using System.Windows.Controls;

namespace VibraScan.Presentation.Views
{
    /// <summary>
    /// Логика взаимодействия для DataImportView.xaml
    /// </summary>
    public partial class DataImportView : UserControl
    {
        public DataImportView()
        {
            InitializeComponent();
            //TestListBox.Items.Add(new { FileName = "vessel_data.xml", StatusText = "Чтение...", IsRemovable = true, IsFaulted = true });
            //TestListBox.Items.Add(new { FileName = "C:\\Users\\Admin\\Downloads\\vbX to Ascent\\vbX - 5A - 2025-12-03 14-56-53.xml", StatusText = "Готово", IsRemovable = false, IsFaulted = false });
        }
    }
}