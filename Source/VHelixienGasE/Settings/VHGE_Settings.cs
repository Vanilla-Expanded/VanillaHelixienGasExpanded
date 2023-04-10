using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace VHelixienGasE
{
    public class VHGE_Settings : ModSettings
    {
        public bool enableDeepDeposits = true;
        public int deepDepositsAmount = 2;
        public int deepDepositsCellCount = 15;

        public bool enableGasGeyser = true;
        public int gasGeyserAmount = 3;

        public bool enableGasOverlay = false;

        const int rowHeight = 24;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableDeepDeposits, "enableDeepDeposits");
            Scribe_Values.Look(ref deepDepositsAmount, "deepDepositsAmount");
            Scribe_Values.Look(ref deepDepositsCellCount, "deepDepositsCellCount");

            Scribe_Values.Look(ref enableGasGeyser, "enableGasGeyser");
            Scribe_Values.Look(ref gasGeyserAmount, "gasGeyserAmount");

            Scribe_Values.Look(ref enableGasOverlay, "enableGasOverlay");
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            var rect = new Rect(inRect.x, inRect.y, inRect.width, rowHeight);
            Widgets.CheckboxLabeled(rect, "VHGE_EnableDeposits".Translate(), ref enableDeepDeposits);
            rect.y += rowHeight;

            if (enableDeepDeposits)
            {
                Widgets.Label(rect.LeftHalf(), "VHGE_DepositsAmount".Translate());
                IntAdjuster(ref deepDepositsAmount, rect.RightHalf(), 1, 1, 12);
                rect.y += rowHeight;

                Widgets.Label(rect.LeftHalf(), "VHGE_DepositsCellCount".Translate());
                IntAdjuster(ref deepDepositsCellCount, rect.RightHalf(), 1, 1, 30);
                rect.y += rowHeight;
            }
        }

        public void IntAdjuster(ref int val, Rect rect, int countChange, int min, int max)
        {
            rect.width = 42f;
            if (Widgets.ButtonText(rect, "-" + countChange))
            {
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
                val -= countChange;
                if (val < min)
                {
                    val = min;
                }
                if (val > max)
                {
                    val = max;
                }
            }
            rect.x += rect.width + 2f;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, val.ToString());
            Text.Anchor = TextAnchor.UpperLeft;
            rect.x += rect.width + 2f;
            if (Widgets.ButtonText(rect, "+" + countChange))
            {
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
                val += countChange;
                if (val < min)
                {
                    val = min;
                }
                if (val > max)
                {
                    val = max;
                }
            }
        }
    }
}
