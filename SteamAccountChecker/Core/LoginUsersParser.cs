using System.Text.RegularExpressions;

namespace SteamAccountChecker.Core;

public class LoginUsersParser
{
    public List<string> ParseLoginUsers(string loginUsersPath)
    {
        var steamIds = new List<string>();

        if (!File.Exists(loginUsersPath))
        {
            return steamIds;
        }

        try
        {
            var vdfText = File.ReadAllText(loginUsersPath);
            
            var pattern = @"7656\d{13}";
            var matches = Regex.Matches(vdfText, pattern);
            
            foreach (Match match in matches)
            {
                var steamId = match.Value;
                if (!steamIds.Contains(steamId))
                {
                    steamIds.Add(steamId);
                }
            }
        }
        catch
        {
        }

        return steamIds;
    }
}
