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
            if (!CellFinderLoose.TryGetRandomCellWith(x => CanScatterAt(x, map), map, 50, out IntVec3 origin))
                Log.Error("Could not find a center cell for infinite helixien gas deposit");

            var def = ThingDefOf.VHGE_Helixien;
            foreach (var cell in GridShapeMaker.IrregularLump(origin, map, 15))
            {
                if (CanScatterAt(cell, map))
                {
                    map.deepResourceGrid.SetAt(cell, def, def.deepCountPerCell);
                    infiniteGasGrid[cell] = true;
                }
            }
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