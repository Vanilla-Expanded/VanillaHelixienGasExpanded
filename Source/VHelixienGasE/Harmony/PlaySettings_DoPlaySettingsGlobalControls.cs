using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VHelixienGasE
{
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
    public static class PlaySettings_DoPlaySettingsGlobalControls
    {
        public static readonly Texture2D ShowGasOverlay = ContentFinder<Texture2D>.Get("UI/Overlays/ShowGasOverlay");

        public static void Postfix(WidgetRow row, bool worldView)
        {
            if (!worldView)
            {
                row.ToggleableIcon(ref VHGE_Mod.settings.enableGasOverlay, ShowGasOverlay, "VHGE_GasOverlayTooltip".Translate());
            }
        }
    }
}
