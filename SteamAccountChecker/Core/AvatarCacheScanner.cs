namespace SteamAccountChecker.Core;

public class AvatarCacheScanner
{
    public List<string> ScanAvatarCache(string avatarCachePath)
    {
        var steamIds = new List<string>();

        try
        {
            var files = Directory.GetFiles(avatarCachePath, "*.png");
            
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                
                if (fileName.Length == 17 && long.TryParse(fileName, out _))
                {
                    steamIds.Add(fileName);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сканирования avatarcache: {ex.Message}");
        }

        return steamIds;
    }
}
