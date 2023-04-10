using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace VHelixienGasE
{
    [StaticConstructorOnStartup]
    public class HelixienGasHandler : MapComponent
    {
        public static Material material = SolidColorMaterials.NewSolidColorMaterial(new Color(0.22f, 0.57f, 0.2f, 0.65f), ShaderDatabase.MetaOverlay);

        public BoolGrid infiniteGasGrid = null;
        public BoolGrid gasGeyserGrid = null;
        public int depositAmount = 0;

        public HelixienGasHandler(Map map) : base(map) { }

        public override void FinalizeInit()
        {
            if (infiniteGasGrid == null && map.ParentFaction == Faction.OfPlayer)
                infiniteGasGrid = new BoolGrid(map);

            if (gasGeyserGrid == null && map.ParentFaction == Faction.OfPlayer)
                gasGeyserGrid = new BoolGrid(map);

            if (VHGE_Mod.settings.enableDeepDeposits)
            {
                for (int i = 0; i < VHGE_Mod.settings.deepDepositsAmount; i++)
                    CreateInfiniteDeposit();
            }
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref infiniteGasGrid, "infiniteGasGrid", null);
            Scribe_Deep.Look(ref gasGeyserGrid, "gasGeyserGrid", null);
            Scribe_Values.Look(ref depositAmount, "depositAmount");
        }

        public override void MapComponentUpdate()
        {
            if (VHGE_Mod.settings.enableGasOverlay)
            {
                foreach (var cell in infiniteGasGrid.ActiveCells)
                    CellRenderer.RenderCell(cell, material);
            }
        }

        private void CreateInfiniteDeposit()
        {
            // Make deposit
            if (!CellFinderLoose.TryGetRandomCellWith(x => CanScatterAt(x, map), map, 50, out IntVec3 origin))
                Log.Error("Could not find a center cell for infinite helixien gas deposit");

            var def = ThingDefOf.VHGE_Helixien;
            foreach (var cell in GridShapeMaker.IrregularLump(origin, map, VHGE_Mod.settings.deepDepositsCellCount))
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
                   && !pos.Impassable(map)
                   && pos.DistanceToEdge(map) > 15;
        }
    }
}