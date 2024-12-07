using System;
using System.Drawing; // Để xử lý ảnh
using System.Windows;
using OpenCvSharp; // Sử dụng OpenCVSharp cho xử lý ảnh
using ActUtlTypeLib; // Thư viện kết nối PLC
using System.Threading;

namespace VisionProgram
{
    public partial class CompareWindow : Window
    {
        private ActUtlType plcConnection = new ActUtlType(); // PLC connection instance
        private string sampleImagePath = "sample_image_path.jpg"; // Đường dẫn ảnh mẫu
        private string capturedImagePath = "captured_image_path.jpg"; // Đường dẫn ảnh chụp thực tế
        private double threshold = 50;  // Ngưỡng phát hiện
        private bool isPLCConnected = false; // Trạng thái kết nối với PLC

        public CompareWindow()
        {
            InitializeComponent();
            ConnectToPLC(); // Kết nối PLC khi mở ứng dụng
        }

        // Kết nối với PLC
        private void ConnectToPLC()
        {
            try
            {
                plcConnection.Open(); // Mở kết nối PLC
                isPLCConnected = true;
                MessageTextBox.Text = "Đã kết nối với PLC!";
            }
            catch (Exception)
            {
                isPLCConnected = false;
                MessageTextBox.Text = "Không thể kết nối với PLC!";
            }
        }

        // Kiểm tra tín hiệu từ PLC và tiến hành chụp và so sánh
        private void CheckPLCAndCompare()
        {
            if (!isPLCConnected)
            {
                MessageTextBox.Text = "Chưa kết nối với PLC.";
                return;
            }

            try
            {
                // Kiểm tra tín hiệu PLC (ví dụ kiểm tra trạng thái của thiết bị "D000")
                short plcSignal = 0;
                plcConnection.GetDevice("D000", out plcSignal);  // Lấy tín hiệu từ PLC (ví dụ thiết bị D000)

                if (plcSignal == 1) // Nếu tín hiệu từ PLC = 1 (bắt đầu quá trình)
                {
                    // Tiến hành chụp ảnh và so sánh
                    CaptureImage();
                    CompareImages();
                }
                else
                {
                    MessageTextBox.Text = "Chưa nhận tín hiệu từ PLC.";
                }
            }
            catch (Exception ex)
            {
                MessageTextBox.Text = $"Lỗi khi kiểm tra tín hiệu: {ex.Message}";
            }
        }

        // Chụp ảnh thực tế qua DinoLite (hoặc phương thức chụp ảnh khác)
        private void CaptureImage()
        {
            // Chụp ảnh (giả sử ảnh được lưu trữ ở đường dẫn sau)
            capturedImagePath = "captured_image_path.jpg";
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
                SendToPLC(matchPercentage);  // Gửi tín hiệu tới PLC
            }
            else
            {
                MessageTextBox.Text = $"Điểm giống nhau: {matchPercentage}% - So sánh thành công.";
            }

            // Hiển thị ảnh mô tả thuật toán (Đánh dấu sự khác biệt)
            HighlightDifferences(diff);
        }

        // Đánh dấu sự khác biệt trên ảnh
        private void HighlightDifferences(Mat diff)
        {
            // Vẽ đường viền (hoặc đánh dấu khác biệt) lên ảnh mô tả thuật toán
            Mat algorithmImage = new Mat(capturedImagePath);  // Lấy ảnh thực tế làm ảnh mô tả thuật toán
            Cv2.Canny(diff, diff, 100, 200);  // Phát hiện biên
            Cv2.BitwiseAnd(algorithmImage, algorithmImage, algorithmImage, diff);
            AlgorithmImage.Source = System.Windows.Media.Imaging.BitmapImage.FromStream(new System.IO.MemoryStream(algorithmImage.ToBytes()));
        }

        // Gửi tín hiệu tới PLC
        private void SendToPLC(double matchPercentage)
        {
            // Gửi tín hiệu PLC nếu điểm giống nhau dưới ngưỡng
            plcConnection.SetDevice("D001", matchPercentage < threshold ? 1 : 0);  // Ví dụ gửi giá trị 1 nếu dưới ngưỡng
        }

        // Hàm kiểm tra tín hiệu từ PLC liên tục
        private void StartPLCCheck()
        {
            Thread checkPLCThread = new Thread(() =>
            {
                while (true)
                {
                    CheckPLCAndCompare();
                    Thread.Sleep(1000); // Kiểm tra tín hiệu mỗi giây
                }
            });
            checkPLCThread.IsBackground = true; // Đảm bảo thread không ảnh hưởng tới UI
            checkPLCThread.Start();
        }

        // Sự kiện khởi động kiểm tra PLC và bắt đầu so sánh
        private void StartComparisonButton_Click(object sender, RoutedEventArgs e)
        {
            StartPLCCheck(); // Bắt đầu kiểm tra tín hiệu PLC
            MessageTextBox.Text = "Đang kiểm tra tín hiệu PLC...";
        }
    }
}
