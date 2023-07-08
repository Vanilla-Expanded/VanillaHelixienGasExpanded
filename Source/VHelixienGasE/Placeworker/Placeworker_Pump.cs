using System.Collections.Generic;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace VHelixienGasE
{
    public class Placeworker_Pump : PlaceWorker_ShowDeepResources
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            var res = map.deepResourceGrid.ThingDefAt(loc);
            if (res != null && res.defName == "VHGE_Helixien")
            {
                // Draw
                if (loc != IntVec3.Invalid)
                {
                    var good = new List<IntVec3>();
                    var treated = new HashSet<IntVec3>();
                    var toCheck = new Queue<IntVec3>();

                    toCheck.Enqueue(loc);
                    treated.Add(loc);

                    while (toCheck.Count > 0)
                    {
                        var temp = toCheck.Dequeue();
                        good.Add(temp);

                        var neighbours = GenAdjFast.AdjacentCellsCardinal(temp);
                        for (int i = 0; i < neighbours.Count; i++)
                        {
                            var n = neighbours[i];
                            if (!treated.Contains(n) && map.deepResourceGrid.ThingDefAt(n) is ThingDef r && r.defName == "VHGE_Helixien")
                            {
                                treated.Add(n);
                                toCheck.Enqueue(n);
                            }
                        }
                    }
                    GenDraw.DrawFieldEdges(good, Color.white);
                }

                return true;
            }

            var geyser = map.thingGrid.ThingAt(loc, ThingDefOf.VHGE_GasGeyser);
            if (geyser != null && geyser.Position == loc)
            {
                return true;
            }

            return (AcceptanceReport)"VHGE_NeedDeepOrGeyser".Translate();
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            base.DrawGhost(def, center, rot, ghostCol, thing);
            if (thing != null && thing.TryGetComp<CompDeepExtractor>() is CompDeepExtractor e && e.lumpCells.Count > 0)
            {
                GenDraw.DrawFieldEdges(e.lumpCells, Color.white);
            }
        }
    }
}