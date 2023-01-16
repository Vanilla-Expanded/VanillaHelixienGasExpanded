using HarmonyLib;
using Verse;

namespace VHelixienGasE
{
    [HarmonyPatch(typeof(DeepResourceGrid), "SetAt")]
    public static class DeepResourceGrid_SetAt
    {
        public static bool Prefix(DeepResourceGrid __instance, IntVec3 c, ThingDef def, int count, Map ___map)
        {
            if (___map.GetComponent<HelixienGasHandler>() is HelixienGasHandler comp && comp.infiniteGasGrid[c])
                return false;

            return true;
        }
    }
}