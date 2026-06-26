using MegaCrit.Sts2.addons.mega_text;
using STS2ExclaimEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ExclaimEverything.Patches;

public sealed class MegaTextReadyPatch : IPatchMethod
{
    public static string PatchId => "mega_text_ready_exclamation";
    public static bool IsCritical => false;
    public static string Description => "Transform scene-initialized Mega text labels after ready!";

    public static ModPatchTarget[] GetTargets()
    {
        return
        [
            new ModPatchTarget(typeof(MegaLabel), nameof(MegaLabel._Ready), Type.EmptyTypes),
            new ModPatchTarget(typeof(MegaRichTextLabel), nameof(MegaRichTextLabel._Ready), Type.EmptyTypes)
        ];
    }

    public static void Postfix(object __instance)
    {
        switch (__instance)
        {
            case MegaLabel label:
                label.SetTextAutoSize(TextTransformer.Transform(label.Text));
                break;
            case MegaRichTextLabel richTextLabel:
                richTextLabel.SetTextAutoSize(TextTransformer.Transform(richTextLabel.Text));
                break;
        }
    }
}
