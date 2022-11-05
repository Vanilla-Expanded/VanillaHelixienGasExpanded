using RimWorld;
using Verse;

namespace VHelixienGasE
{
    public class HelixienGasHandler : MapComponent
    {
        internal BoolGrid infiniteGasGrid = null;

        public HelixienGasHandler(Map map) : base(map) { }

        public override void FinalizeInit()
        {
            if (infiniteGasGrid == null && map.ParentFaction == Faction.OfPlayer)
            {
                infiniteGasGrid = new BoolGrid(map);
                // Make deposit    
                if (!CellFinderLoose.TryFindRandomNotEdgeCellWith(10, x => CanScatterAt(x, map), map, out IntVec3 result))
                    Log.Error("Could not find a center cell for infinite helixien gas deposit");

                var def = TDefOf.VHGE_Helixien;
                foreach (var intVec3 in GridShapeMaker.IrregularLump(result, map, 15))
                {
                    if (CanScatterAt(intVec3, map) && !intVec3.InNoBuildEdgeArea(map))
                    {
                        map.deepResourceGrid.SetAt(intVec3, def, def.deepCountPerCell);
                        infiniteGasGrid[intVec3] = true;
                    }
                }

                Find.LetterStack.ReceiveLetter("VHGE_InfiniteDepositLabel".Translate(), "VHGE_InfiniteDeposit".Translate(), LetterDefOf.PositiveEvent, new LookTargets(result, map));
            }
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref infiniteGasGrid, "infiniteGasGrid", null);
        }

        private bool CanScatterAt(IntVec3 pos, Map map)
        {
            int index = CellIndicesUtility.CellToIndex(pos, map.Size.x);
            var terrainDef = map.terrainGrid.TerrainAt(index);
            return (terrainDef == null || !terrainDef.IsWater || terrainDef.passability != Traversability.Impassable) && terrainDef.affordances.Contains(ThingDefOf.DeepDrill.terrainAffordanceNeeded) && !map.deepResourceGrid.GetCellBool(index);
        }
    }
}
