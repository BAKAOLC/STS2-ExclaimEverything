using MegaCrit.Sts2.addons.mega_text;
using STS2ExclaimEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ExclaimEverything.Patches;

public sealed class MegaTextAutoSizePatch : IPatchMethod
{
    public static string PatchId => "mega_text_auto_size_exclamation";
    public static string Description => "Transform Mega text labels before display!";

    public static ModPatchTarget[] GetTargets()
    {
        return
        [
            new ModPatchTarget(typeof(MegaLabel), nameof(MegaLabel.SetTextAutoSize), [typeof(string)]),
            new ModPatchTarget(typeof(MegaRichTextLabel), nameof(MegaRichTextLabel.SetTextAutoSize), [typeof(string)])
        ];
    }

    public static void Prefix(ref string text)
    {
        text = TextTransformer.Transform(text);
    }
}
