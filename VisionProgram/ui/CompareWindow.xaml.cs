using System;
using System.Drawing; // Để xử lý ảnh
using System.Windows;
using OpenCvSharp; // Sử dụng OpenCVSharp cho xử lý ảnh
using ActUtlTypeLib; // Thư viện kết nối PLC
using System.IO;
using System.Windows.Media.Imaging;

namespace VisionProgram
{
    public partial class CompareWindow : System.Windows.Window
    {
        private string sampleImagePath;
        private string capturedImagePath;
        private double threshold = 50;  // Ngưỡng phát hiện

        private ActUtlType plcConnection; // PLC connection instance

        public CompareWindow()
        {
            InitializeComponent();
            LoadSampleImage(); // Tải ảnh mẫu khi mở cửa sổ
        }

        // Tải ảnh mẫu
        private void LoadSampleImage()
        {
            sampleImagePath = "D:\\Analysis\\defective-product\\clean_screen.png";  // Đặt đường dẫn ảnh mẫu
            SampleImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(sampleImagePath));
        }

        // Xử lý sự kiện khi kiểm tra kết nối PLC
        private void CheckPLCConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //plcConnection.Open(); // Mở kết nối PLC
                MessageTextBox.Text = "Đã kết nối với PLC!";
                //plcConnection.Close();
            }
            catch (Exception)
            {
                MessageTextBox.Text = "Kết nối với PLC thất bại!";
            }
        }

        // Xử lý sự kiện khi bắt đầu so sánh
        private void StartComparisonButton_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra kết nối PLC và thực hiện hành động
            CheckPLCConnectionButton_Click(sender, e);  // Kiểm tra kết nối trước khi so sánh
            if (1 == 1) // Nếu kết nối thành công
            {
                CaptureImage();
                CompareImages();
            }
            else
            {
                MessageTextBox.Text = "Không có kết nối PLC. Không thể so sánh!";
            }
        }

        // Chụp ảnh thực tế qua DinoLite
        private void CaptureImage()
        {
            // Chụp ảnh (giả sử ảnh được lưu trữ ở đường dẫn sau)
            capturedImagePath = "D:\\Analysis\\defective-product\\screen_with_smudge.png";
            CapturedImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(capturedImagePath));
            MessageTextBox.Text = "Ảnh đã được chụp.";
        }

        // So sánh ảnh mẫu với ảnh thực tế
        private void CompareImages()
        {
            // Đọc ảnh mẫu và ảnh thực tế
            Mat sampleImage = Cv2.ImRead(sampleImagePath, ImreadModes.Grayscale);
            Mat capturedImage = Cv2.ImRead(capturedImagePath, ImreadModes.Grayscale);

            // Tiến hành so sánh (Sử dụng phương pháp SOF (Structure of Features) hoặc phương pháp so sánh đơn giản như MSE)
            Mat diff = new Mat();
            Cv2.Absdiff(sampleImage, capturedImage, diff);

            // Tính toán sự khác biệt
            double similarity = Cv2.CountNonZero(diff);  // Số điểm khác biệt
            double totalPixels = sampleImage.Rows * sampleImage.Cols;

            double matchPercentage = (1 - similarity / totalPixels) * 100;

            // Hiển thị kết quả
            if (matchPercentage < threshold)
            {
                MessageTextBox.Text = $"Khác biệt nhiều! Điểm giống nhau: {matchPercentage}% - Gửi thông tin tới PLC.";
                SendToPLC(matchPercentage);  // Gửi tín hiệu đến PLC
            }
            else
            {
                MessageTextBox.Text = $"Điểm giống nhau: {matchPercentage}% - So sánh thành công.";
            }

            // Hiển thị ảnh mô tả thuật toán (Đánh dấu sự khác biệt)
            HighlightDifferences(diff);
        }

        // Đánh dấu sự khác biệt trên ảnh
        public void HighlightDifferences(Mat diff)
        {
            // Convert Mat to Bitmap
            var stream = new MemoryStream(diff.ToBytes());

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            // Hiển thị ảnh mô tả thuật toán (dựng lại ảnh từ kết quả so sánh)
            AlgorithmImage.Source = bitmapImage;
        }

        // Gửi tín hiệu tới PLC
        private void SendToPLC(double matchPercentage)
        {
            // Gửi tín hiệu PLC nếu điểm giống nhau dưới ngưỡng
            plcConnection.SetDevice("D000", matchPercentage < threshold ? 1 : 0);  // Ví dụ gửi giá trị 1 nếu dưới ngưỡng
        }
    }
}
