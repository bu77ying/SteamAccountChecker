using System.Text.Json;
using System.Xml.Linq;
using SteamAccountChecker.Models;

namespace SteamAccountChecker.Services;

public class VacCheckerService
{
    private readonly HttpClient _httpClient;

    public VacCheckerService()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        };
        
        _httpClient = new HttpClient(handler);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36 Edg/145.0.0.0");
    }

    public async Task<VacStatus?> CheckVacStatus(string steamId64)
    {
        try
        {
            var url = $"https://steamcommunity.com/profiles/{steamId64}/?xml=1";
            var response = await _httpClient.GetStringAsync(url);
            
            var doc = XDocument.Parse(response);
            var profile = doc.Root;

            if (profile == null)
                return null;

            var vacBannedElement = profile.Element("vacBanned");
            var gameBannedElement = profile.Element("isGameBanned");
            var isLimitedElement = profile.Element("isLimitedAccount");
            var tradeBanStateElement = profile.Element("tradeBanState");
            
            var vacBanned = vacBannedElement?.Value == "1";
            var gameBanned = gameBannedElement?.Value == "1";
            var isLimited = isLimitedElement?.Value == "1";
            var tradeBanState = tradeBanStateElement?.Value ?? "None";
            
            var daysSinceLastBan = 0;
            var daysSinceElement = profile.Element("daysSinceLastBan");
            if (daysSinceElement != null && int.TryParse(daysSinceElement.Value, out var days))
            {
                daysSinceLastBan = days;
            }

            return new VacStatus
            {
                VacBanned = vacBanned,
                GameBanned = gameBanned,
                DaysSinceLastBan = daysSinceLastBan
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при проверке {steamId64}: {ex.Message}");
        }

        return null;
    }
}
