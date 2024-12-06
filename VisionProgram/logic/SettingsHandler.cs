using Newtonsoft.Json;
using System.IO;

public class SettingsHandler
{
    private const string SettingsFilePath = "settings.json"; // Đường dẫn file JSON

    // Lưu cấu hình
    public static void SaveSettings(AppSettings settings)
    {
        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(SettingsFilePath, json);
    }

    // Tải cấu hình
    public static AppSettings LoadSettings()
    {
        if (File.Exists(SettingsFilePath))
        {
            string json = File.ReadAllText(SettingsFilePath);
            return JsonConvert.DeserializeObject<AppSettings>(json);
        }
        return new AppSettings(); // Trả về mặc định nếu file không tồn tại
    }
}
