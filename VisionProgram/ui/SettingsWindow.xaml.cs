using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using ActUtlTypeLib;

namespace VisionProgram
{
    public partial class SettingsWindow : Window
    {
        private string sampleImagePath;
        private string saveFolderPath;
        private double threshold = 50;

        private ActUtlType plcConnection;

        public SettingsWindow()
        {
            InitializeComponent();
            ThresholdSlider.ValueChanged += ThresholdSlider_ValueChanged;
            LoadSettings(); // Load cài đặt khi mở cửa sổ
        }

        // Xử lý sự kiện khi chọn ảnh mẫu
        private void SelectSampleImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                sampleImagePath = openFileDialog.FileName;
                SampleImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(sampleImagePath));
            }
        }

        // Xử lý sự kiện khi chọn thư mục lưu ảnh
        private void SelectSaveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Khởi tạo đối tượng OpenFolderDialog
            var openFolderDialog = new OpenFolderDialog();

            // Mở hộp thoại chọn thư mục
            if (openFolderDialog.ShowDialog() == true)
            {
                saveFolderPath = openFolderDialog.FolderName; // Lấy đường dẫn thư mục
                SaveFolderTextBox.Text = saveFolderPath;  // Hiển thị đường dẫn
                
            }
        }



        // Xử lý sự kiện thay đổi giá trị ngưỡng phát hiện
        private void ThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            threshold = e.NewValue;
            ThresholdValue.Text = threshold.ToString("0");
        }

        // Xử lý sự kiện kiểm tra kết nối Mitsubishi PLC
        private void CheckPLCConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            plcConnection = new ActUtlType(); // Thư viện PLC
            try
            {
                plcConnection.Open();
                PLCConnectionStatus.Text = "Đã kết nối";
                plcConnection.Close();
            }
            catch (Exception)
            {
                PLCConnectionStatus.Text = "Kết nối thất bại";
            }
        }

        // Lưu cài đặt vào file JSON
        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new
            {
                SampleImagePath = sampleImagePath,
                SaveFolderPath = saveFolderPath,
                Threshold = threshold
            };

            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);

            // Tạo file JSON nếu không tồn tại
            string settingsFilePath = "settings.json";
            File.WriteAllText(settingsFilePath, json);

            MessageBox.Show("Cài đặt đã được lưu!");
        }

        // Tải cài đặt từ file JSON
        private void LoadSettings()
        {
            string settingsFilePath = "settings.json";
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                var settings = JsonConvert.DeserializeObject<dynamic>(json);

                sampleImagePath = settings.SampleImagePath;
                saveFolderPath = settings.SaveFolderPath;
                threshold = settings.Threshold;

                if (!string.IsNullOrEmpty(sampleImagePath))
                {
                    SampleImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(sampleImagePath));
                }
                ThresholdSlider.Value = threshold;
                ThresholdValue.Text = threshold.ToString("0");
            }
        }
    }
}
