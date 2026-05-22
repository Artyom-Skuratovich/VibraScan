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
            TestListBox.Items.Add(new { FileName = "vessel_data.xml", StatusText = "Чтение...", IsRemovable = true, IsFaulted = true });
        }
    }
}