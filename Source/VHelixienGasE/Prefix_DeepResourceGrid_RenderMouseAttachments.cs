using HarmonyLib;
using UnityEngine;
using Verse;

namespace VHelixienGasE
{
    [HarmonyPatch(typeof(DeepResourceGrid), "RenderMouseAttachments")]
    public static class Prefix_DeepResourceGrid_RenderMouseAttachments
    {
        public static bool Prefix(DeepResourceGrid __instance, Map ___map)
        {
            IntVec3 c = UI.MouseCell();
            if (HarmonyInit.GetHandlerFor(___map) is HelixienGasHandler comp && comp.infiniteGasGrid[c])
            {
                if (!c.InBounds(___map))
                    return false;

                var thingDef = TDefOf.VHGE_Helixien;

                Vector2 uiPosition = c.ToVector3().MapToUIPosition();
                GUI.color = Color.white;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                float num2 = (float)(((double)UI.CurUICellSize() - 27.0) / 2.0);
                Rect rect = new Rect(uiPosition.x + num2, uiPosition.y - UI.CurUICellSize() + num2, 27f, 27f);
                Widgets.ThingIcon(rect, thingDef);
                Widgets.Label(new Rect(rect.xMax + 4f, rect.y, 999f, 29f), "DeepResourceRemaining".Translate(NamedArgumentUtility.Named(thingDef, "RESOURCE"), "infinite".Named("COUNT")));
                Text.Anchor = TextAnchor.UpperLeft;

                return false;
            }

            return true;
        }
    }
}
