using STS2ExclaimEverything.Data;

namespace STS2ExclaimEverything.Utils;

internal static class ExclaimSettingsService
{
    public static bool AppendMissingTerminalExclamation
    {
        get
        {
            try
            {
                return ModDataStore.GetSettings().AppendMissingTerminalExclamation;
            }
            catch
            {
                return true;
            }
        }
    }
}