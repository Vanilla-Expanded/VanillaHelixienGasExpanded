using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace VHelixienGasE
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        private static readonly Dictionary<Map, HelixienGasHandler> cache = new Dictionary<Map, HelixienGasHandler>();

        static HarmonyInit()
        {
            Harmony harmonyInstance = new Harmony("VE.HelixienGas");
            harmonyInstance.PatchAll();
        }

        public static HelixienGasHandler GetHandlerFor(Map map)
        {
            if (cache.ContainsKey(map))
                return cache[map];

            var comp = map.GetComponent<HelixienGasHandler>();
            cache.Add(map, comp);
            return comp;
        }
    }
}
