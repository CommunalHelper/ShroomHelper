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

        public static void Load() {
            Everest.Events.Level.OnLoadEntity += Level_OnLoadEntity;
            Everest.Events.Level.OnTransitionTo += Level_TransitionTo;
            On.Celeste.Player.StartDash += Player_StartDash;
            On.Celeste.Player.RefillDash += Player_RefillDash;
            On.Celeste.Player.UseRefill += Player_UseRefill;
            Everest.Events.Player.OnDie += Player_OnDie;
        }

        public static void Unload() {
            Everest.Events.Level.OnLoadEntity -= Level_OnLoadEntity;
            Everest.Events.Level.OnTransitionTo -= Level_TransitionTo;
            On.Celeste.Player.StartDash -= Player_StartDash;
            On.Celeste.Player.RefillDash -= Player_RefillDash;
            On.Celeste.Player.UseRefill -= Player_UseRefill;
            Everest.Events.Player.OnDie -= Player_OnDie;
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

        private static bool Level_OnLoadEntity(Level level, LevelData levelData, Vector2 offset, EntityData entityData) {
            if (entityData.Name == "ShroomHelper/OneDashWingedStrawberry") {
                return !level.Session.StartedFromBeginning || ShroomHelperModule.Session.dashedTwice;
            }

            return false; // false loads the entity, true de-spawns it
        }

        // only set session dashedTwice data on room transitions
        private static void Level_TransitionTo(Level level, LevelData next, Vector2 direction) {
            if (ShroomHelperModule.Session.brokeDashLimitInRoom) {
                ShroomHelperModule.Session.dashedTwice = true;
                ShroomHelperModule.Session.brokeDashLimitInRoom = false;
            }
        }

        private static void Player_OnDie(Player player) {
            ShroomHelperModule.Session.beforeRefillDashCount = 0;
            ShroomHelperModule.Session.brokeDashLimitInRoom = false;
        }

        private static int Player_StartDash(On.Celeste.Player.orig_StartDash orig, Player self) {
            orig(self);
            ShroomHelperModule.Session.beforeRefillDashCount++;
            if (ShroomHelperModule.Session.beforeRefillDashCount > 1) {
                ShroomHelperModule.Session.brokeDashLimitInRoom = false;
            }

            return Player.StDash;
        }

        private static bool Player_RefillDash(On.Celeste.Player.orig_RefillDash orig, Player self) {
            ShroomHelperModule.Session.beforeRefillDashCount = 0;
            return orig(self);
        }

        private static bool Player_UseRefill(On.Celeste.Player.orig_UseRefill orig, Player self, bool twoDashes) {
            ShroomHelperModule.Session.beforeRefillDashCount = 0;
            return orig(self, twoDashes);
        }

        private void OnDash(Vector2 dir) {
            if (ShroomHelperModule.Session.brokeDashLimitInRoom) {
                if (!baseData.Get<bool>("flyingAway") && !WaitingOnSeeds && Follower.Leader?.Entity is not Player) {
                    Depth = -1000000;
                    Add(new Coroutine(FlyAwayRoutine()));
                    baseData.Set("flyingAway", true);

                    if (despawnFromSessionIfDashedTwiceInSameRoom) {
                        ShroomHelperModule.Session.dashedTwice = true;
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
