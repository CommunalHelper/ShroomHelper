using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.ShroomHelper.Triggers {
    [CustomEntity("ShroomHelper/TimeModulationTrigger")]
    public class TimeModulationTrigger : Trigger {
        public float timeFrom;
        public float timeTo;
        public PositionModes PositionMode;
        public bool completeAreaOnFrozen;
        public bool persistent;
        public bool destroyOnLeave;

        private EntityID id;

        public TimeModulationTrigger(EntityData data, Vector2 offset, EntityID eid)
            : base(data, offset) {
            id = eid;
            timeFrom = data.Float("timeFrom", 1f);
            timeTo = data.Float("timeTo", 1f);
            PositionMode = data.Enum("positionMode", PositionModes.NoEffect);
            destroyOnLeave = data.Bool("destroyOnLeave", false);
            persistent = data.Bool("persistent", false);
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
            float time = MathHelper.Lerp(timeFrom, timeTo, GetPositionLerp(player, PositionMode));
            Engine.TimeRate = time;
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);

            if (destroyOnLeave) {
                Remove();
            }
        }

        public void Remove() {
            RemoveSelf();

            if (persistent) {
                Level level = SceneAs<Level>();
                level.Session.DoNotLoad.Add(id);
            }
        }
    }
}