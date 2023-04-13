using System;
using System.Collections.Generic;
using System.Linq;
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

        public static List<IntVec3> IrregularLumpWith(Predicate<IntVec3> validator, IntVec3 center, Map map, int numCells)
        {
            var lumpCells = new List<IntVec3>();
            // Populate cells list
            for (int i = 0; i < numCells * 3; i++)
            {
                var cell = center + GenRadial.RadialPattern[i];
                if (cell.InBounds(map) && validator(cell)) // Respect validator
                {
                    lumpCells.Add(cell);
                }
            }

            int NumNeighbors(IntVec3 sq)
            {
                var count = 0;
                for (int k = 0; k < 4; k++)
                {
                    var cell = sq + GenAdj.CardinalDirections[k];
                    if (lumpCells.Contains(cell))
                    {
                        count++;
                    }
                }
                return count;
            }

            while (lumpCells.Count > numCells)
            {
                var fewestNeighbors = 99;
                for (int j = 0; j < lumpCells.Count; j++)
                {
                    var arg = lumpCells[j];
                    var num = NumNeighbors(arg);
                    if (num < fewestNeighbors)
                    {
                        fewestNeighbors = num;
                    }
                }

                var source = lumpCells.Where(sq => NumNeighbors(sq) == fewestNeighbors).ToList();
                lumpCells.Remove(source.RandomElement());
            }

            return lumpCells;
        }

        [DebugAction("Pipe System", "Re-roll infinite gas deposits", false, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ReRollDeposit()
        {
            var map = Find.CurrentMap;
            if (map == null)
                return;

            var comp = map.GetComponent<HelixienGasHandler>();
            if (comp == null)
                return;

            // Remove deposits
            var cells = comp.infiniteGasGrid.ActiveCells.ToList();
            comp.infiniteGasGrid = null;
            foreach (var cell in cells)
            {
                map.deepResourceGrid.SetAt(cell, null, 0);
            }
            // Recreate
            comp.FinalizeInit();
        }
    }
}