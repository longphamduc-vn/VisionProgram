using System.Configuration;
using System.Data;
using System.Windows;

namespace VisionProgram
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Đọc cấu hình từ file JSON khi ứng dụng khởi động
            string configFilePath = "config.json"; // Đường dẫn đến file cấu hình
            ConfigurationManager.LoadConfiguration(configFilePath);
        }
    }

}
