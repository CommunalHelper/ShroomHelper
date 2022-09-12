using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections;

namespace Celeste.Mod.ShroomHelper.Entities {
    [CustomEntity("ShroomHelper/OneDashWingedStrawberry")]
    public class OneDashWingedStrawberry : Strawberry {
        public bool getFlag;
        public float collectTimer = 0f;

        protected DynamicData baseData;

        private readonly bool despawnFromSessionIfDashedTwiceInSameRoom;

        public OneDashWingedStrawberry(EntityData data, Vector2 offset, EntityID gid)
            : base(data, offset, gid) {
            baseData = new DynamicData(typeof(Strawberry), this);
            baseData.Set("Winged", true);
            baseData.Set("Golden", true);
            despawnFromSessionIfDashedTwiceInSameRoom = data.Bool("despawnFromSessionIfDashedTwiceInSameRoom", false);
            Remove(Get<DashListener>());
            Add(new DashListener { OnDash = OnDash });
        }

        public override void Update() {
            base.Update();
            if (!baseData.Get<bool>("collected")) {
                if (Follower.Leader?.Entity is Player leaderPlayer && !leaderPlayer.Dead) {
                    getFlag = true;
                }

                if (getFlag) {
                    collectTimer += Engine.DeltaTime;
                    if (collectTimer > 0.5f) {
                        OnCollect();
                    }
                }
            }
        }

        private void OnDash(Vector2 dir) {
            if (ShroomHelperModule.ShroomSession.brokeDashLimitInRoom) {
                if (!baseData.Get<bool>("flyingAway") && !WaitingOnSeeds && Follower.Leader?.Entity is not Player) {
                    Depth = -1000000;
                    Add(new Coroutine(FlyAwayRoutine()));
                    baseData.Set("flyingAway", true);

                    if (despawnFromSessionIfDashedTwiceInSameRoom) {
                        ShroomHelperModule.ShroomSession.dashedTwice = true;
                    }
                }
            }
        }

        private IEnumerator FlyAwayRoutine() {
            baseData.Get<Wiggler>("rotateWiggler").Start();
            baseData.Set("flapSpeed", -200f);
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.25f, start: true);
            tween.OnUpdate = delegate (Tween t) {
                baseData.Set("flapSpeed", MathHelper.Lerp(-200f, 0f, t.Eased));
            };
            Add(tween);
            yield return 0.1f;
            Audio.Play("event:/game/general/strawberry_laugh", Position);
            yield return 0.2f;
            if (!Follower.HasLeader) {
                Audio.Play("event:/game/general/strawberry_flyaway", Position);
            }

            tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.5f, start: true);
            tween.OnUpdate = delegate (Tween t) {
                baseData.Set("flapSpeed", MathHelper.Lerp(0f, -200f, t.Eased));
            };
            Add(tween);
        }
    }
}
