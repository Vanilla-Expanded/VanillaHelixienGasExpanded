using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PipeSystem;
using UnityEngine;
using Verse;
using Verse.Noise;
using static HarmonyLib.Code;
using static RimWorld.FleshTypeDef;

namespace VHelixienGasE
{
    [HarmonyPatch(typeof(CompDeepExtractor), "TryProducePortion")]
    public static class CompDeepExtractor_TryProducePortion
    {
        public static bool Prefix(CompDeepExtractor __instance)
        {
            var loc = __instance.parent.Position;
            var geyser = __instance.parent.Map.thingGrid.ThingAt(loc, ThingDefOf.VHGE_GasGeyser);

            if (geyser != null && geyser.Position == loc && __instance.PipeNet is PipeNet net && net.storages.Count >= 1)
            {
                var available = (int)net.AvailableCapacity;
                __instance.noCapacity = available <= 1;

                if (__instance.noCapacity == false)
                {
                    net.DistributeAmongStorage(30 > available ? available : 30);
                    __instance.StartSustainer();

                    if (!__instance.cycleOver) __instance.cycleOver = true;
                }
                else
                {
                    __instance.EndSustainer();
                }

                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(CompDeepExtractor), "PostSpawnSetup")]
    public static class CompDeepExtractor_PostSpawnSetup
    {
        public static void Postfix(CompDeepExtractor __instance)
        {
            var loc = __instance.parent.Position;
            var geyser = __instance.parent.Map.thingGrid.ThingAt(loc, ThingDefOf.VHGE_GasGeyser);

            if (geyser != null && geyser.Position == loc && geyser is Building_GasGeyser gasGeyser)
            {
                gasGeyser.harvester = (Building)__instance.parent;
            }
        }
    }
}
