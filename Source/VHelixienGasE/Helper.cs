using System.Collections.Generic;
using Verse;

namespace VHelixienGasE
{
    public static class Helper
    {
        private static readonly Dictionary<Map, HelixienGasHandler> cache = new Dictionary<Map, HelixienGasHandler>();

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