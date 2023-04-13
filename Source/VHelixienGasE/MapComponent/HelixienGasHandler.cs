using System;
using System.Collections.Generic;
using System.Linq;
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

        const int MinSpacing = 25;
        const int MinEdgeDistance = 15;

        public HelixienGasHandler(Map map) : base(map) { }

        public override void FinalizeInit()
        {
            InitDeposits();
            InitGeysers();
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref infiniteGasGrid, "infiniteGasGrid", null);
        }

        public override void MapComponentUpdate()
        {
            if (VHGE_Mod.settings.enableGasOverlay)
            {
                foreach (var cell in infiniteGasGrid.ActiveCells)
                    CellRenderer.RenderCell(cell, material);
            }
        }

        public void InitDeposits()
        {
            if (map.ParentFaction != Faction.OfPlayer)
                return;

            // Init bool grid
            infiniteGasGrid = new BoolGrid(map);
            // Spawn deposit(s)
            if (VHGE_Mod.settings.enableDeepDeposits)
            {
                var def = ThingDefOf.VHGE_Helixien;

                for (int i = 0; i < VHGE_Mod.settings.deepDepositsAmount; i++)
                {
                    // Find starting cell
                    if (!CellFinderLoose.TryGetRandomCellWith(x => CanScatterAt(x, map), map, 50, out IntVec3 origin))
                    {
                        Log.Error("Couldn't find a starting cell to spawn infinite helixien gas deposit");
                        return;
                    }
                    // Make deposit
                    foreach (var cell in Helper.IrregularLumpWith(x => CanScatterAt(x, map), origin, map, VHGE_Mod.settings.deepDepositsCellCount))
                    {
                        map.deepResourceGrid.SetAt(cell, def, def.deepCountPerCell);
                        infiniteGasGrid[cell] = true;
                    }
                }
            }
        }

        private bool CanScatterAt(IntVec3 pos, Map map)
        {
            int index = CellIndicesUtility.CellToIndex(pos, map.Size.x);
            var terrainDef = map.terrainGrid.TerrainAt(index);

            return pos.InBounds(map)
                   && pos.DistanceToEdge(map) > 15
                   && pos.GetFirstThing(map, RimWorld.ThingDefOf.SteamGeyser) == null
                   && (terrainDef == null || !terrainDef.IsWater || terrainDef.passability != Traversability.Impassable)
                   && terrainDef.affordances.Contains(TerrainAffordanceDefOf.Medium)
                   && !map.deepResourceGrid.GetCellBool(index)
                   && !pos.Impassable(map);
        }

        public void InitGeysers()
        {
            if (map.TileInfo.WaterCovered || !VHGE_Mod.settings.enableGasGeyser)
                return;

            var steamGeysers = map.listerThings.ThingsMatching(ThingRequest.ForDef(RimWorld.ThingDefOf.SteamGeyser));
            var geysersSpots = new List<IntVec3>();
            for (int i = 0; i < steamGeysers.Count; i++)
            {
                geysersSpots.Add(steamGeysers[i].Position);
            }

            var usedSpots = new List<IntVec3>();
            for (int i = 0; i < VHGE_Mod.settings.gasGeyserAmount; i++)
            {
                if (CellFinderLoose.TryFindRandomNotEdgeCellWith(MinEdgeDistance, x => CanSpawnAt(x, map, usedSpots, geysersSpots), map, out IntVec3 result))
                {
                    usedSpots.Add(result);
                    GenSpawn.Spawn(ThingDefOf.VHGE_GasGeyser, result, map);
                }
            }
        }

        private bool CanSpawnAt(IntVec3 x, Map map, List<IntVec3> usedSpots, List<IntVec3> geysersSpots)
        {
            var occupiedCells = GenAdj.OccupiedRect(x, Rot4.North, new IntVec2(2, 2)).ToList();
            for (int i = 0; i < occupiedCells.Count; i++)
            {
                var cell = occupiedCells[i];
                if (!cell.GetTerrain(map).affordances.Contains(TerrainAffordanceDefOf.Heavy)
                    || !cell.Standable(map)
                    || NearUsedSpot(cell, usedSpots, MinSpacing * MinSpacing)
                    || NearUsedSpot(cell, geysersSpots, 4 * 4))
                    return false;
            }

            return true;
        }

        private bool NearUsedSpot(IntVec3 c, List<IntVec3> usedSpots, int spacingSquared)
        {
            for (int i = 0; i < usedSpots.Count; i++)
            {
                if ((float)(usedSpots[i] - c).LengthHorizontalSquared <= spacingSquared)
                {
                    return true;
                }
            }
            return false;
        }
    }
}