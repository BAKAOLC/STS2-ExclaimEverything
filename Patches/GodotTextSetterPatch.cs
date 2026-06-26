using Godot;
using HarmonyLib;
using STS2ExclaimEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ExclaimEverything.Patches;

public sealed class GodotTextSetterPatch : IPatchMethod
{
    public static string PatchId => "godot_text_setter_exclamation";
    public static bool IsCritical => false;
    public static string Description => "Transform generic Godot label text before display!";

    public static ModPatchTarget[] GetTargets()
    {
        return
        [
            new ModPatchTarget(typeof(Button), nameof(Button.Text), null, true, MethodType.Setter),
            new ModPatchTarget(typeof(Label), nameof(Label.Text), null, true, MethodType.Setter),
            new ModPatchTarget(typeof(RichTextLabel), nameof(RichTextLabel.Text), null, true, MethodType.Setter)
        ];
    }

    public static void Prefix(ref string value)
    {
        value = TextTransformer.Transform(value);
    }
}
