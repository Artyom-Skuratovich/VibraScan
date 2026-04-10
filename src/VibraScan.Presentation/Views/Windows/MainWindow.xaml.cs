using System.Windows;
using VibraScan.Presentation.Common.Interfaces;

namespace VibraScan.Presentation.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}