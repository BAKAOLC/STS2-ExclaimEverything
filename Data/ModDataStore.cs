using STS2ExclaimEverything.Utils;
using STS2RitsuLib;
using STS2RitsuLib.Utils.Persistence;

namespace STS2ExclaimEverything.Data;

internal static class ModDataStore
{
    public const string SettingsKey = "settings";

    private const string SettingsFileName = "settings.json";

    private static readonly STS2RitsuLib.Data.ModDataStore Store =
        STS2RitsuLib.Data.ModDataStore.For(Const.ModId);

    public static void Initialize()
    {
        using (RitsuLibFramework.BeginModDataRegistration(Const.ModId))
        {
            Store.Register(
                SettingsKey,
                SettingsFileName,
                SaveScope.Global,
                () => new ExclaimSettings(),
                true);
        }
    }

    public static ExclaimSettings GetSettings()
    {
        return Store.Get<ExclaimSettings>(SettingsKey);
    }
}