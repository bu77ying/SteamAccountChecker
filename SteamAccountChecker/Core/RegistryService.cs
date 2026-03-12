using Microsoft.Win32;

namespace SteamAccountChecker.Core;

public class RegistryService
{
    public string? GetSteamInstallPath()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            if (key != null)
            {
                var path = key.GetValue("SteamPath") as string;
                if (!string.IsNullOrEmpty(path))
                    return path.Replace("/", "\\");
            }

            using var key64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");
            if (key64 != null)
            {
                var path = key64.GetValue("InstallPath") as string;
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            using var key32 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");
            if (key32 != null)
            {
                var path = key32.GetValue("InstallPath") as string;
                if (!string.IsNullOrEmpty(path))
                    return path;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка чтения реестра: {ex.Message}");
        }

        return null;
    }
}
