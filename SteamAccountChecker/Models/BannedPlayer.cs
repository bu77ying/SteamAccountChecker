namespace SteamAccountChecker.Models;

public class BannedPlayer
{
    public string Nickname { get; set; } = string.Empty;
    public string? SteamId { get; set; }
    public string? Reason { get; set; }
    public string? BanDate { get; set; }
    public int DurationDays { get; set; }
    public long DurationSeconds { get; set; }
    public BanSource Source { get; set; }
}

public enum BanSource
{
    Fear,
    Yooma
}
