using Verse;

namespace VHelixienGasE
{
    public class CompProperties_CompDestroyOnSpawn : CompProperties
    {
        public CompProperties_CompDestroyOnSpawn()
        {
            compClass = typeof(CompDestroyOnSpawn);
        }
    }

    internal class CompDestroyOnSpawn : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            parent.DeSpawn(DestroyMode.Vanish);
        }
    }
}