using SteamAccountChecker.Core;

namespace SteamAccountChecker.Services;

public class SteamScannerService
{
    private readonly string _steamPath;
    private readonly UserDataScanner _userDataScanner;
    private readonly AvatarCacheScanner _avatarCacheScanner;
    private readonly ConfigVdfParser _configVdfParser;
    private readonly LoginUsersParser _loginUsersParser;

    public SteamScannerService(string steamPath)
    {
        _steamPath = steamPath;
        _userDataScanner = new UserDataScanner();
        _avatarCacheScanner = new AvatarCacheScanner();
        _configVdfParser = new ConfigVdfParser();
        _loginUsersParser = new LoginUsersParser();
    }

    public HashSet<string> CollectAllSteamIds()
    {
        var steamIds = new HashSet<string>();

        var userDataPath = Path.Combine(_steamPath, "userdata");
        if (Directory.Exists(userDataPath))
        {
            var ids = _userDataScanner.ScanUserData(userDataPath);
            foreach (var id in ids)
                steamIds.Add(id);
        }

        var avatarCachePath = Path.Combine(_steamPath, "config", "avatarcache");
        if (Directory.Exists(avatarCachePath))
        {
            var ids = _avatarCacheScanner.ScanAvatarCache(avatarCachePath);
            foreach (var id in ids)
                steamIds.Add(id);
        }

        var configVdfPath = Path.Combine(_steamPath, "config", "config.vdf");
        if (File.Exists(configVdfPath))
        {
            var ids = _configVdfParser.ParseConfigVdf(configVdfPath);
            foreach (var id in ids)
                steamIds.Add(id);
        }

        var loginUsersPath = Path.Combine(_steamPath, "config", "loginusers.vdf");
        if (File.Exists(loginUsersPath))
        {
            var ids = _loginUsersParser.ParseLoginUsers(loginUsersPath);
            foreach (var id in ids)
                steamIds.Add(id);
        }

        return steamIds;
    }
}
