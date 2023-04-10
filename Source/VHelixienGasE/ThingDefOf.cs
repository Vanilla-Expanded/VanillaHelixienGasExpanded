using RimWorld;
using Verse;

namespace VHelixienGasE
{
    [DefOf]
    public static class ThingDefOf
    {
        public static ThingDef VHGE_Helixien;
        public static ThingDef VHGE_GasGeyser;

        static ThingDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
    }
}