using System.Windows;
using ActUtlTypeLib;

namespace VisionProgram
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Xử lý sự kiện khi nhấn nút "So sánh"
        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            CompareWindow compareWindow = new CompareWindow();
            compareWindow.Show();
        }

        // Xử lý sự kiện khi nhấn nút "Cài đặt"
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

    }
}
