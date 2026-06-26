using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2ExclaimEverything.Data;
using STS2ExclaimEverything.Patches;
using STS2ExclaimEverything.Settings;
using STS2RitsuLib;
using STS2RitsuLib.Patching.Core;

namespace STS2ExclaimEverything;

[ModInitializer(nameof(Initialize))]
public static class Main
{
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(Const.ModId);

    public static bool IsModActive { get; private set; }

    public static void Initialize()
    {
        Logger.Info($"Mod ID: {Const.ModId}");
        Logger.Info($"Version: {Const.Version}");
        Logger.Info("Initializing mod...");

        try
        {
            var patcher = RitsuLibFramework.CreatePatcher(Const.ModId, "main");
            RegisterMainPatches(patcher);

            if (!RitsuLibFramework.ApplyRequiredPatcher(patcher, () => IsModActive = false))
            {
                Logger.Error("Mod initialization failed: Critical patch(es) failed to apply");
                return;
            }

            IsModActive = true;
            ModDataStore.Initialize();
            ModSettingsBootstrap.Initialize();
            Logger.Info("Mod initialization complete - Mod is now ACTIVE");
        }
        catch (Exception ex)
        {
            Logger.Error($"Mod initialization failed with exception: {ex.Message}");
            Logger.Error($"Stack trace: {ex.StackTrace}");
            IsModActive = false;
        }
    }

    private static void RegisterMainPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<LocalizedFormattedTextPatch>();
        patcher.RegisterPatch<MegaTextAutoSizePatch>();
        patcher.RegisterPatch<GodotTextSetterPatch>();
        patcher.RegisterPatch<CanvasItemDrawStringPatch>();
    }
}