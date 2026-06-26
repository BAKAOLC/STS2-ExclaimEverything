using MegaCrit.Sts2.Core.Localization;
using STS2ExclaimEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ExclaimEverything.Patches;

public sealed class LocalizedFormattedTextPatch : IPatchMethod
{
    public static string PatchId => "localized_formatted_text_exclamation";
    public static string Description => "Transform formatted localized text!";

    public static ModPatchTarget[] GetTargets()
    {
        return [new ModPatchTarget(typeof(LocString), nameof(LocString.GetFormattedText), Type.EmptyTypes)];
    }

    public static void Postfix(ref string __result)
    {
        __result = TextTransformer.TransformPeriodsOnly(__result);
    }
}
