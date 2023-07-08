using System.Linq;
using HarmonyLib;
using PipeSystem;
using Verse;

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
                __instance.noCapacity = available == 0;

                if (__instance.noCapacity)
                {
                    __instance.EndSustainer();
                }
                else
                {
                    net.DistributeAmongStorage(30 > available ? available : 30, out _);
                    __instance.StartSustainer();

                    if (!__instance.cycleOver) __instance.cycleOver = true;
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
                __instance.lumpCells = gasGeyser.OccupiedRect().Cells.ToList();
            }
        }
    }
}