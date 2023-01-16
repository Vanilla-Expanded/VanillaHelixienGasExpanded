using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VHelixienGasE
{
    public class HelixienGasHandler : MapComponent
    {
        public BoolGrid infiniteGasGrid = null;

        private int depositAmount = 0;

        public HelixienGasHandler(Map map) : base(map)
        {
        }

        public override void FinalizeInit()
        {
            if (infiniteGasGrid == null && map.ParentFaction == Faction.OfPlayer)
                infiniteGasGrid = new BoolGrid(map);

            CreateInfiniteDeposit();
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref infiniteGasGrid, "infiniteGasGrid", null);
            Scribe_Values.Look(ref depositAmount, "depositAmount");
        }

        private void CreateInfiniteDeposit()
        {
            // Make deposit
            if (!CellFinderLoose.TryGetRandomCellWith(x => CanScatterAt(x, map), map, 10000, out IntVec3 origin))
                Log.Error("Could not find a center cell for infinite helixien gas deposit");

            var def = TDefOf.VHGE_Helixien;
            foreach (var cell in IrregularLump(origin, 15, map))
            {
                if (!cell.InNoBuildEdgeArea(map) && CanScatterAt(cell, map))
                {
                    map.deepResourceGrid.SetAt(cell, def, def.deepCountPerCell);
                    infiniteGasGrid[cell] = true;
                }
            }
        }

        private List<IntVec3> IrregularLump(IntVec3 center, int numCells, Map map)
        {
            var lumpCells = new List<IntVec3>();
            var count = 0;

            while (count < numCells)
            {
                var cell = count > 0 ? lumpCells.RandomElement() : center;
                cell += GenAdj.CardinalDirections[Rand.RangeInclusive(0, 3)];

                if (!lumpCells.Contains(cell) && CanScatterAt(cell, map))
                {
                    lumpCells.Add(cell);
                    count++;
                }
            }
            return lumpCells;
        }

        private bool CanScatterAt(IntVec3 pos, Map map)
        {
            int index = CellIndicesUtility.CellToIndex(pos, map.Size.x);
            var terrainDef = map.terrainGrid.TerrainAt(index);

            return pos.InBounds(map)
                   && !pos.InNoBuildEdgeArea(map)
                   && (terrainDef == null || !terrainDef.IsWater || terrainDef.passability != Traversability.Impassable)
                   && terrainDef.affordances.Contains(TerrainAffordanceDefOf.Medium)
                   && !map.deepResourceGrid.GetCellBool(index)
                   && !pos.Impassable(map);
        }
    }
}