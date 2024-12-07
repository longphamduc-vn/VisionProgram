using Newtonsoft.Json;
using System.IO;

public static class AppConfig
{
    public static AppSettings Settings { get; set; }
}

public class ConfigurationManager
{
    public static void LoadConfiguration(string configFilePath)
    {
        try
        {
            // Đọc nội dung của file JSON
            var json = File.ReadAllText(configFilePath);

            // Giải mã (Deserialize) JSON thành đối tượng AppSettings
            AppConfig.Settings = JsonConvert.DeserializeObject<AppSettings>(json);
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu không thể đọc file JSON
            Console.WriteLine($"Lỗi khi đọc cấu hình: {ex.Message}");
        }
    }
    public static void SaveConfiguration(string configFilePath)
    {
        try
        {
            // Chuyển đối tượng AppSettings thành chuỗi JSON
            var json = JsonConvert.SerializeObject(AppConfig.Settings, Formatting.Indented);

            // Ghi chuỗi JSON vào file
            File.WriteAllText(configFilePath, json);
        }
        catch (Exception ex)
        {
            // Xử lý lỗi khi lưu file
            Console.WriteLine($"Lỗi khi lưu cấu hình: {ex.Message}");
        }
    }
}
