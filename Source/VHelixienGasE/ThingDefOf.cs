using RimWorld;
using Verse;

namespace VHelixienGasE
{
    [DefOf]
    public static class ThingDefOf
    {
        public static ThingDef VHGE_Helixien;

        static ThingDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
    }
}