using System.Collections.Generic;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;
using System;

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
           
            if (thing != null && thing.TryGetComp<PipeSystem.CompDeepExtractor>() is PipeSystem.CompDeepExtractor e && e?.lumpCells!=null&&e.lumpCells.Count > 0)
            {
                GenDraw.DrawFieldEdges(e.lumpCells, Color.white);
            }
        }

        // Probably not needed, may include it just in case.
        public override bool ForceAllowPlaceOver(BuildableDef other) => other == ThingDefOf.VHGE_GasGeyser;

        public override void DrawMouseAttachments(BuildableDef def)
        {
            var list = Find.CurrentMap.listerThings.ThingsOfDef(ThingDefOf.VHGE_GasGeyser);
            for (var i = 0; i < list.Count; i++)
                TargetHighlighter.Highlight(list[i]);
        }
    }
}