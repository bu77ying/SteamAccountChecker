namespace SteamAccountChecker.Models;

public class VacStatus
{
    public bool VacBanned { get; set; }
    public bool GameBanned { get; set; }
    public int DaysSinceLastBan { get; set; }
}
