using Godot;
using STS2ExclaimEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ExclaimEverything.Patches;

public sealed class CanvasItemDrawStringPatch : IPatchMethod
{
    public static string PatchId => "canvas_item_draw_string_exclamation";
    public static bool IsCritical => false;
    public static string Description => "Transform custom drawn CanvasItem text before display!";

    public static ModPatchTarget[] GetTargets()
    {
        return
        [
            new ModPatchTarget(typeof(CanvasItem), nameof(CanvasItem.DrawString),
            [
                typeof(Font),
                typeof(Vector2),
                typeof(string),
                typeof(HorizontalAlignment),
                typeof(float),
                typeof(int),
                typeof(Color?),
                typeof(TextServer.JustificationFlag),
                typeof(TextServer.Direction),
                typeof(TextServer.Orientation)
            ]),
            new ModPatchTarget(typeof(CanvasItem), nameof(CanvasItem.DrawString),
            [
                typeof(Font),
                typeof(Vector2),
                typeof(string),
                typeof(HorizontalAlignment),
                typeof(float),
                typeof(int),
                typeof(Color?),
                typeof(TextServer.JustificationFlag),
                typeof(TextServer.Direction),
                typeof(TextServer.Orientation),
                typeof(float)
            ])
        ];
    }

    public static void Prefix(ref string text)
    {
        text = TextTransformer.Transform(text);
    }
}
