using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace VHelixienGasE
{
    public class GenStep_SpawnGasGeyser : GenStep
    {
        public override int SeedPart => 825516005;

        public int minSpacing;
        public int minEdgeDistance;

        protected List<IntVec3> usedSpots = new List<IntVec3>();

        public override void Generate(Map map, GenStepParams parms)
        {
            if (map.TileInfo.WaterCovered || !VHGE_Mod.settings.enableGasGeyser)
                return;

            for (int i = 0; i < VHGE_Mod.settings.gasGeyserAmount; i++)
            {
                if (CellFinderLoose.TryFindRandomNotEdgeCellWith(minEdgeDistance, (IntVec3 x) => CanSpawnAt(x, map), map, out IntVec3 result))
                {
                    usedSpots.Add(result);
                    GenSpawn.Spawn(ThingDefOf.VHGE_GasGeyser, result, map);
                }
            }
        }

        private bool CanSpawnAt(IntVec3 x, Map map)
        {
            return x.GetTerrain(map).affordances.Contains(TerrainAffordanceDefOf.Heavy) && x.Standable(map) && !NearUsedSpot(x);
        }

        private bool NearUsedSpot(IntVec3 c)
        {
            for (int i = 0; i < usedSpots.Count; i++)
            {
                if ((float)(usedSpots[i] - c).LengthHorizontalSquared <= minSpacing * minSpacing)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
