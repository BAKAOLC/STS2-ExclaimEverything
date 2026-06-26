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

            var appendPureNumericTerminalBinding = ModSettingsBindings.WithDefault(
                ModSettingsBindings.Global<ExclaimSettings, bool>(
                    Const.ModId,
                    ModDataStore.SettingsKey,
                    settings => settings.AppendPureNumericTerminalExclamation,
                    (settings, value) => settings.AppendPureNumericTerminalExclamation = value),
                () => false);

            var uppercaseConvertibleCharactersBinding = ModSettingsBindings.WithDefault(
                ModSettingsBindings.Global<ExclaimSettings, bool>(
                    Const.ModId,
                    ModDataStore.SettingsKey,
                    settings => settings.UppercaseConvertibleCharacters,
                    (settings, value) => settings.UppercaseConvertibleCharacters = value),
                () => false);

            RitsuLibFramework.RegisterModSettings(Const.ModId, page => page
                .WithModDisplayName(T("Exclaim Everything!", "全都感叹号！"))
                .WithTitle(T("Settings", "设置"))
                .WithDescription(T(
                    "Adjust text exclamation behavior! Some already-created text may require reopening the screen or restarting the game before changes apply!",
                    "调整文本感叹号替换行为！部分已经创建的文本可能需要重新打开界面或重启游戏后才会生效！"))
                .AddSection("text", section => section
                    .WithTitle(T("Text", "文本"))
                    .AddToggle(
                        "append_missing_terminal_exclamation",
                        T("Append missing exclamation marks", "为无标点行尾补感叹号"),
                        appendMissingTerminalBinding,
                        T(
                            "When a displayed line has words but no punctuation at the line end or text end, append an exclamation mark! Pure numeric UI labels are ignored!",
                            "当显示文本中的一行包含文字、但行尾或文本末尾没有标点时，补上感叹号！纯数字界面标签会被忽略！"))
                    .AddToggle(
                        "append_pure_numeric_terminal_exclamation",
                        T("Append marks to pure numbers", "为纯数字补感叹号"),
                        appendPureNumericTerminalBinding,
                        T(
                            "When enabled, pure numeric labels such as 123, 1.5, and 50% can also receive missing exclamation marks!",
                            "开启后，123、1.5、50% 这样的纯数字标签也可以补上缺失的感叹号！"))
                    .AddToggle(
                        "uppercase_convertible_characters",
                        T("Uppercase convertible characters", "大写可转换字符"),
                        uppercaseConvertibleCharactersBinding,
                        T(
                            "When enabled, every displayed character that has an uppercase form is converted to uppercase!",
                            "开启后，显示文本中所有能转换为大写的字符都会变成大写！"))));

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
