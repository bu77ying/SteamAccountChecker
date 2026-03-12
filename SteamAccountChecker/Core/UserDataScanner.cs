namespace SteamAccountChecker.Core;

public class UserDataScanner
{
    public List<string> ScanUserData(string userDataPath)
    {
        var steamIds = new List<string>();

        try
        {
            var directories = Directory.GetDirectories(userDataPath);
            
            foreach (var dir in directories)
            {
                var dirName = Path.GetFileName(dir);
                
                if (long.TryParse(dirName, out var steam3Id))
                {
                    var steamId64 = ConvertSteam3ToSteamId64(steam3Id);
                    steamIds.Add(steamId64);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сканирования userdata: {ex.Message}");
        }

        return steamIds;
    }

    private string ConvertSteam3ToSteamId64(long steam3Id)
    {
        const long baseId = 76561197960265728L;
        return (steam3Id + baseId).ToString();
    }
}
