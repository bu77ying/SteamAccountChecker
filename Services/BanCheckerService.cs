using System.Text.Json;
using System.Net.WebSockets;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using SteamAccountChecker.Models;

namespace SteamAccountChecker.Services;

public class BanCheckerService
{
    private readonly HttpClient _httpClient;
    private const string FearApiUrl = "https://api.fearproject.ru/punishments/search";
    private const string YoomaWsUrl = "wss://yooma.su/api";

    public BanCheckerService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "YoomaChecker/1.0");
    }

    public async Task<List<BannedPlayer>> CheckBans(string? nickname = null, string? steamId = null)
    {
        var bannedPlayers = new List<BannedPlayer>();

        try
        {
            var fearBanned = await FetchFearBanned(nickname, steamId);
            bannedPlayers.AddRange(fearBanned);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [Fear] Ошибка: {ex.Message}");
        }

        try
        {
            var yoomaBanned = await FetchYoomaBannedWs(steamId);
            bannedPlayers.AddRange(yoomaBanned);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [Yooma] Ошибка: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"  [Yooma] Детали: {ex.InnerException.Message}");
        }

        return bannedPlayers;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "JSON serialization types are preserved")]
    private async Task<List<BannedPlayer>> FetchFearBanned(string? nickname, string? steamId)
    {
        if (string.IsNullOrEmpty(steamId))
            return new List<BannedPlayer>();

        var url = $"{FearApiUrl}?q={Uri.EscapeDataString(steamId)}&page=1&limit=50&type=1";

        var response = await _httpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Fear API error: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        
        if (string.IsNullOrWhiteSpace(content))
            return new List<BannedPlayer>();
        
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = JsonContext.Default
        };
        
        var data = JsonSerializer.Deserialize<FearApiResponse>(content, options);

        return data?.Punishments?
            .Where(p => p.Status == 1)
            .Select(p => {
                var created = DateTimeOffset.FromUnixTimeSeconds(p.Created);
                var durationSeconds = p.Duration > 0 ? p.Duration : -1;
                var durationDays = durationSeconds > 0 ? (int)(durationSeconds / 86400.0) : -1;
                
                return new BannedPlayer
                {
                    Nickname = p.Name ?? string.Empty,
                    SteamId = p.Steamid,
                    Reason = p.Reason,
                    BanDate = created.ToString("yyyy-MM-dd HH:mm:ss"),
                    DurationDays = durationDays,
                    DurationSeconds = durationSeconds,
                    Source = BanSource.Fear
                };
            }).ToList() ?? new List<BannedPlayer>();
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "JSON serialization types are preserved")]
    private async Task<List<BannedPlayer>> FetchYoomaBannedWs(string? steamId)
    {
        if (string.IsNullOrEmpty(steamId))
            return new List<BannedPlayer>();

        using var ws = new ClientWebSocket();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        
        try
        {
            await ws.ConnectAsync(new Uri(YoomaWsUrl), cts.Token);

            var requestJson = $"{{\"type\":\"get_punishments\",\"page\":1,\"punish_type\":0,\"search\":\"{steamId}\",\"mobile\":false}}";
            var requestBytes = Encoding.UTF8.GetBytes(requestJson);
            await ws.SendAsync(new ArraySegment<byte>(requestBytes), WebSocketMessageType.Text, true, cts.Token);

            var buffer = new byte[1024 * 16];
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            var responseJson = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (responseJson.Contains("\"type\":\"get_type\""))
            {
                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                responseJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            }

            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                TypeInfoResolver = JsonContext.Default
            };

            var response = JsonSerializer.Deserialize<YoomaWsResponse>(responseJson, options);

            if (response?.Punishments == null || response.Punishments.Count == 0)
                return new List<BannedPlayer>();

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            return response.Punishments
                .Where(p => p.Unpunish_Admin_Id == null 
                    && (p.Expires == 0 || p.Expires > now)
                    && p.Admin_Steam_Id != steamId)
                .Select(p => {
                    var created = DateTimeOffset.FromUnixTimeSeconds(p.Created);
                    var durationSeconds = p.Expires > 0 ? p.Expires - p.Created : -1;
                    var durationDays = durationSeconds > 0 ? (int)(durationSeconds / 86400.0) : -1;
                    
                    return new BannedPlayer
                    {
                        Nickname = p.Name ?? string.Empty,
                        SteamId = p.Steamid,
                        Reason = p.Reason,
                        BanDate = created.ToString("yyyy-MM-dd HH:mm:ss"),
                        DurationDays = durationDays,
                        DurationSeconds = durationSeconds,
                        Source = BanSource.Yooma
                    };
                }).ToList();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
