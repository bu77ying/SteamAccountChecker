namespace SteamAccountChecker.Models;

public class FearApiResponse
{
    public List<FearPunishment>? Punishments { get; set; }
}

public class FearPunishment
{
    public int Id { get; set; }
    public string? Steamid { get; set; }
    public string? Name { get; set; }
    public string? Admin { get; set; }
    public string? Admin_Steamid { get; set; }
    public string? Reason { get; set; }
    public int Status { get; set; }
    public long Duration { get; set; }
    public long Created { get; set; }
    public long Expires { get; set; }
}

public class YoomaWsResponse
{
    public string? Type { get; set; }
    public List<YoomaPunishment>? Punishments { get; set; }
}

public class YoomaPunishment
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Steamid { get; set; }
    public string? Reason { get; set; }
    public long Created { get; set; }
    public long Expires { get; set; }
    public int Punish_Type { get; set; }
    public int? Unpunish_Admin_Id { get; set; }
    public string? Admin_Steam_Id { get; set; }
}

public class YoomaApiResponse
{
    public List<YoomaBan>? Bans { get; set; }
}

public class YoomaBan
{
    public string? Nickname { get; set; }
    public string? Player_Name { get; set; }
    public string? SteamId { get; set; }
    public string? Steam_Id { get; set; }
    public string? Reason { get; set; }
    public string? Ban_Reason { get; set; }
    public string? BanDate { get; set; }
    public string? Banned_At { get; set; }
}
