using UnityEngine;
using Verse;

namespace VHelixienGasE
{
    public class PlaceWorker_AlwaysShowDeepResources : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Find.CurrentMap.deepResourceGrid.MarkForDraw();
        }
    }
}