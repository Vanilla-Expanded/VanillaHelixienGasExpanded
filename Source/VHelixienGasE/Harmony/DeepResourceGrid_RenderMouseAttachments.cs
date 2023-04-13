using HarmonyLib;
using UnityEngine;
using Verse;

namespace VHelixienGasE
{
    [HarmonyPatch(typeof(DeepResourceGrid), "RenderMouseAttachments")]
    public static class DeepResourceGrid_RenderMouseAttachments
    {
        public static bool Prefix(Map ___map)
        {
            var c = UI.MouseCell();
            if (c.InBounds(___map) && Helper.GetHandlerFor(___map) is HelixienGasHandler comp && comp.infiniteGasGrid != null && comp.infiniteGasGrid[c])
            {
                if (!c.InBounds(___map))
                    return false;

                var thingDef = ThingDefOf.VHGE_Helixien;

                var uiPosition = c.ToVector3().MapToUIPosition();

                GUI.color = Color.white;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;

                var num = (UI.CurUICellSize() - 27) / 2;
                Rect rect = new Rect(uiPosition.x + num, uiPosition.y - UI.CurUICellSize() + num, 27f, 27f);
                Widgets.ThingIcon(rect, thingDef);
                Widgets.Label(new Rect(rect.xMax + 4f, rect.y, 999f, 29f), "DeepResourceRemaining".Translate(NamedArgumentUtility.Named(thingDef, "RESOURCE"), "infinite".Named("COUNT")));

                Text.Anchor = TextAnchor.UpperLeft;

                return false;
            }

            return true;
        }
    }
}