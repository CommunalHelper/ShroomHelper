// Celeste.LightFadeTrigger
using Celeste;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;
using System.Collections;
using System.Reflection;

namespace Celeste.Mod.ShroomHelper.Triggers
{
    [CustomEntity("ShroomHelper/TimeModulationTrigger")]
    public class TimeModulationTrigger : Trigger
    {
        public float timeFrom;

        public float timeTo;

        public PositionModes PositionMode;

        public bool completeAreaOnFrozen;

        public bool persistent;

        public bool destroyOnLeave;

        private EntityID id;

        private static FieldInfo screenWipeDuration = typeof(ScreenWipe).GetField("Duration", BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public);

        public TimeModulationTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            id = new EntityID(data.Level.Name, data.ID);
            timeFrom = data.Float("timeFrom", 1f);
            timeTo = data.Float("timeTo", 1f);
            PositionMode = data.Enum("positionMode", PositionModes.NoEffect);
            destroyOnLeave = data.Bool("destroyOnLeave", false);
            persistent = data.Bool("persistent", false);
        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);
            float time = MathHelper.Lerp(timeFrom, timeTo, GetPositionLerp(player, PositionMode));
            Engine.TimeRate = time;
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);

            if (destroyOnLeave)
            {
                Remove();
            }
        }

        public void Remove()
        {
            RemoveSelf();

            if (persistent)
            {
                Level level = SceneAs<Level>();
                level.Session.DoNotLoad.Add(id);
            }
        }
    }
}