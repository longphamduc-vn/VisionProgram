using System;
using System.Drawing; // Để xử lý ảnh
using System.Windows;
using OpenCvSharp; // Sử dụng OpenCVSharp cho xử lý ảnh
using ActUtlTypeLib;
using System.IO;
using System.Windows.Media.Imaging; // Thư viện kết nối PLC
using Microsoft.Win32;
using System.Windows.Input;
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
        using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Input;

private void CapturedImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Mở hộp thoại chọn file để chọn ảnh cần so sánh
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

        // Nếu người dùng chọn file, tiến hành so sánh
        if (openFileDialog.ShowDialog() == true)
        {
            string selectedImagePath = openFileDialog.FileName;

            // Kiểm tra xem ảnh đã chọn có tồn tại không
            if (File.Exists(selectedImagePath))
            {
                // Tải ảnh thực tế và ảnh mẫu
                BitmapImage selectedImage = LoadImageFromFile(selectedImagePath);
                BitmapImage sampleImage = LoadImageFromFile("C:\\path\\to\\sample_image.jpg");  // Đảm bảo đường dẫn ảnh mẫu chính xác

                // Hiển thị ảnh đã chọn
                CapturedImage.Source = selectedImage;

                // Tiến hành so sánh ảnh
                CompareImages(sampleImage, selectedImage);
            }
            else
            {
                MessageBox.Show("Ảnh không tồn tại.");
            }
        }
    }

    private BitmapImage LoadImageFromFile(string imagePath)
    {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(imagePath);
        bitmapImage.EndInit();
        return bitmapImage;
    }

    private void CompareImages(BitmapImage sampleImage, BitmapImage capturedImage)
    {
        // Tiến hành so sánh ảnh mẫu với ảnh thực tế tại đây
        // Bạn có thể thực hiện việc so sánh hình ảnh (ví dụ sử dụng các thuật toán như SSIM, MSE, hoặc các thư viện hỗ trợ so sánh ảnh)

        // Sau khi so sánh, hiển thị kết quả và điểm so sánh
        double similarityScore = 0.95;  // Ví dụ điểm so sánh (cần thay đổi theo thuật toán thực tế)

        // Hiển thị kết quả so sánh trên ảnh thực tế
        MessageTextBox.Text = $"Điểm so sánh: {similarityScore * 100}%";

        // Kiểm tra nếu điểm giống nhau thấp hơn ngưỡng thì gửi thông tin tới PLC (giả sử ngưỡng là 85%)
        if (similarityScore < 0.85)
        {
            // Gửi thông tin đến PLC (giả sử bạn có chức năng gửi tín hiệu đến PLC ở đây)
            SendSignalToPLC();
        }
    }

    private void SendSignalToPLC()
    {
        // Mã gửi tín hiệu tới PLC (bạn cần thực hiện theo cách của bạn)
        MessageTextBox.Text += "\nĐang gửi tín hiệu đến PLC...";
    }

    // Tải ảnh mẫu
    private void LoadSampleImage()
        {

            sampleImagePath = "C:\\Users\\hoan\\Downloads\\hinh-nen-vu-tru-la-gi.jpg";  // Đặt đường dẫn ảnh mẫu
            SampleImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(sampleImagePath));
        }

        // Xử lý sự kiện khi kiểm tra kết nối PLC
        private void CheckPLCConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                plcConnection = new ActUtlType();
                plcConnection.Open(); // Mở kết nối PLC
                MessageTextBox.Text = "Đã kết nối với PLC!";
                plcConnection.Close();
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
            //if (plcConnection.Status == 1) // Nếu kết nối thành công
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
        private void CapturedImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Mở hộp thoại chọn file để chọn ảnh cần so sánh
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            // Nếu người dùng chọn file, tiến hành so sánh
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;

                // Kiểm tra xem ảnh đã chọn có tồn tại không
                if (File.Exists(selectedImagePath))
                {
                    // Tải ảnh thực tế và ảnh mẫu
                    BitmapImage selectedImage = LoadImageFromFile(selectedImagePath);
                    BitmapImage sampleImage = LoadImageFromFile("C:\\path\\to\\sample_image.jpg");  // Đảm bảo đường dẫn ảnh mẫu chính xác

                    // Hiển thị ảnh đã chọn
                    CapturedImage.Source = selectedImage;

                    // Tiến hành so sánh ảnh
                    CompareImages(sampleImage, selectedImage);
                }
                else
                {
                    MessageBox.Show("Ảnh không tồn tại.");
                }
            }
        }

    }
}
