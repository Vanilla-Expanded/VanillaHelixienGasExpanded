using RimWorld;
using Verse;

namespace VHelixienGasE
{
    [DefOf]
    public static class TDefOf
    {
        public static ThingDef VHGE_Helixien;

        static TDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(TDefOf));
    }
}
