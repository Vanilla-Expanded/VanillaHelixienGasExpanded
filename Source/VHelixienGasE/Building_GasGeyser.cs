using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VHelixienGasE
{
    public class Building_GasGeyser : Building
    {
        public Building harvester;

        private IntermittentGasSprayer gasSprayer;
        private Sustainer spraySustainer;
        private int spraySustainerStartTick = -999;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            gasSprayer = new IntermittentGasSprayer(this)
            {
                startSprayCallback = StartSpray,
                endSprayCallback = EndSpray
            };
        }

        private void StartSpray()
        {
            spraySustainer = SoundDefOf.GeyserSpray.TrySpawnSustainer(new TargetInfo(Position, Map));
            spraySustainerStartTick = Find.TickManager.TicksGame;
        }

        private void EndSpray()
        {
            if (spraySustainer != null)
            {
                spraySustainer.End();
                spraySustainer = null;
            }
        }

        public override void Tick()
        {
            if (harvester == null)
            {
                gasSprayer.GasSprayerTick();
            }
            if (spraySustainer != null && Find.TickManager.TicksGame > spraySustainerStartTick + 1000)
            {
                Log.Message("Gas geyser spray sustainer still playing after 1000 ticks. Force-ending.");
                spraySustainer.End();
                spraySustainer = null;
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
                yield return gizmo;

            yield return new Command_Action()
            {
                defaultLabel = "VHGE_FillGeyser".Translate(),
                defaultDesc = "VHGE_FillGeyserDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/VHGE_PlugHole"),
                action = () =>
                {
                    if (harvester == null)
                    {
                        DeSpawn();
                    }
                }
            };
        }
    }

    public class IntermittentGasSprayer
    {
        private readonly Thing parent;

        private int ticksUntilSpray = 500;

        private int sprayTicksLeft;

        public Action startSprayCallback;

        public Action endSprayCallback;

        private const int MinTicksBetweenSprays = 500;
        private const int MaxTicksBetweenSprays = 2000;
        private const int MinSprayDuration = 200;
        private const int MaxSprayDuration = 500;
        private const float SprayThickness = 0.6f;

        public IntermittentGasSprayer(Thing parent)
        {
            this.parent = parent;
        }

        public void GasSprayerTick()
        {
            if (sprayTicksLeft > 0)
            {
                sprayTicksLeft--;
                if (Rand.Value < SprayThickness)
                {
                    ThrowGasPuffUp(parent.TrueCenter(), parent.Map);
                }
                if (Find.TickManager.TicksGame % 20 == 0)
                {
                    GenTemperature.PushHeat(parent, 40f);
                }
                if (sprayTicksLeft <= 0)
                {
                    endSprayCallback?.Invoke();
                    ticksUntilSpray = Rand.RangeInclusive(MinTicksBetweenSprays, MaxTicksBetweenSprays);
                }
                return;
            }

            ticksUntilSpray--;
            if (ticksUntilSpray <= 0)
            {
                startSprayCallback?.Invoke();
                sprayTicksLeft = Rand.RangeInclusive(MinSprayDuration, MaxSprayDuration);
            }
        }

        public void ThrowGasPuffUp(Vector3 loc, Map map)
        {
            if (loc.ToIntVec3().ShouldSpawnMotesAt(map))
            {
                var data = FleckMaker.GetDataStatic(loc + new Vector3(Rand.Range(-0.02f, 0.02f), 0f, Rand.Range(-0.02f, 0.02f)), map, FleckDefOf.AirPuff, 1.5f);
                data.rotationRate = Rand.RangeInclusive(-240, 240);
                data.velocityAngle = Rand.Range(-45, 45);
                data.velocitySpeed = Rand.Range(1.2f, 1.5f);
                data.instanceColor = new Color(0.22f, 0.57f, 0.2f);
                map.flecks.CreateFleck(data);
            }
        }
    }
}
