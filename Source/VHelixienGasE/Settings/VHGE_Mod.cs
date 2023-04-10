using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VHelixienGasE
{
    public class VHGE_Mod : Mod
    {
        public static VHGE_Settings settings;

        public VHGE_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<VHGE_Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect) => settings.DoSettingsWindowContents(inRect);

        public override string SettingsCategory() => "VHGE_Name".Translate();

        public override void WriteSettings()
        {
            base.WriteSettings();
            settings.WriteSettings();
        }
    }
}
