using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.IO;
using ExpenseTracker.Models;

public class UserService
{
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static readonly string FolderPath = Path.Combine(DesktopPath, "JsonData");
    private static readonly string FilePath = Path.Combine(FolderPath, "appdata.json");  

    // Load AppData from json to obj
    public AppData LoadData()
    {
        if (!File.Exists(FilePath))
            return new AppData(); 

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();  
    }

    // Save AppData to JSON
    public void SaveData(AppData appData)
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath); 
        }

        var json = JsonSerializer.Serialize(appData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);  
    }

    // Hash password using SHA256
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash); 
    }

    // Validate password by comparing the hashed password
    public bool ValidatePassword(string inputPassword, string storedPassword)
    {
        var hashedInputPassword = HashPassword(inputPassword);
        return hashedInputPassword == storedPassword; 
    }

    // Load Users from the file
    public List<User> LoadUsers()
    {
        var appData = LoadData();
        return appData.Users;
    }

    // Save Users to the file
    public void SaveUsers(List<User> users)
    {
        var appData = LoadData();
        appData.Users = users;
        SaveData(appData);
    }
}
