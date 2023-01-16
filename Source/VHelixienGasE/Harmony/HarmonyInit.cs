using HarmonyLib;
using Verse;

namespace VHelixienGasE
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            Harmony harmonyInstance = new Harmony("VE.HelixienGas");
            harmonyInstance.PatchAll();
        }
    }
}