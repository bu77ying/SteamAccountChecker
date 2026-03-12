using System.Text.RegularExpressions;

namespace SteamAccountChecker.Core;

public class ConfigVdfParser
{
    public List<string> ParseConfigVdf(string configVdfPath)
    {
        var steamIds = new List<string>();

        if (!File.Exists(configVdfPath))
        {
            return steamIds;
        }

        try
        {
            var vdfText = File.ReadAllText(configVdfPath);
            
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
