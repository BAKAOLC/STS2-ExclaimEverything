using MegaCrit.Sts2.Core.Localization;
using STS2ExclaimEverything.Data;
using STS2ExclaimEverything.Utils;
using STS2RitsuLib;
using STS2RitsuLib.Settings;

namespace STS2ExclaimEverything.Settings;

internal static class ModSettingsBootstrap
{
    private static readonly Lock InitLock = new();
    private static bool _initialized;

    public static void Initialize()
    {
        lock (InitLock)
        {
            if (_initialized)
                return;

            var appendMissingTerminalBinding = ModSettingsBindings.WithDefault(
                ModSettingsBindings.Global<ExclaimSettings, bool>(
                    Const.ModId,
                    ModDataStore.SettingsKey,
                    settings => settings.AppendMissingTerminalExclamation,
                    (settings, value) => settings.AppendMissingTerminalExclamation = value),
                () => true);

            RitsuLibFramework.RegisterModSettings(Const.ModId, page => page
                .WithModDisplayName(T("Exclaim Everything!", "全都感叹号！"))
                .WithTitle(T("Settings", "设置"))
                .WithDescription(T(
                    "Adjust text exclamation behavior.",
                    "调整文本感叹号替换行为。"))
                .AddSection("text", section => section
                    .WithTitle(T("Text", "文本"))
                    .AddToggle(
                        "append_missing_terminal_exclamation",
                        T("Append missing exclamation marks", "为无标点行尾补感叹号"),
                        appendMissingTerminalBinding,
                        T(
                            "When a displayed line has words but no punctuation at the line end or text end, append an exclamation mark. Pure numeric UI labels are ignored.",
                            "当显示文本中的一行包含文字、但行尾或文本末尾没有标点时，补上感叹号。纯数字界面标签会被忽略。"))));

            _initialized = true;
        }
    }

    private static ModSettingsText T(string english, string simplifiedChinese)
    {
        return ModSettingsText.Dynamic(() => IsSimplifiedChinese() ? simplifiedChinese : english);
    }

    private static bool IsSimplifiedChinese()
    {
        try
        {
            return string.Equals(LocManager.Instance?.Language, "zhs", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}