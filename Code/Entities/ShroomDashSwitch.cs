using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.ShroomHelper.Entities {
    [CustomEntity("ShroomHelper/ShroomDashSwitch")]
    public class ShroomDashSwitch : DashSwitch {
        private readonly bool refillDash;
        private readonly bool doubleDashRefill;
        private readonly bool isWindTrigger;
        private readonly WindController.Patterns windPattern;
        private readonly DynamicData baseData;
        private BloomPoint bloom;

        public ShroomDashSwitch(Vector2 position, Sides side, bool persistent, EntityID id, bool refillDash, bool doubleDashRefill, bool isWindTrigger, WindController.Patterns windPattern)
            : base(position, side, persistent, allGates: false, id, GetSpriteName(refillDash, doubleDashRefill)) {
            this.refillDash = refillDash;
            this.doubleDashRefill = doubleDashRefill;
            this.isWindTrigger = isWindTrigger;
            this.windPattern = windPattern;

            baseData = new DynamicData(typeof(DashSwitch), this);
        }

        public ShroomDashSwitch(EntityData data, Vector2 offset) : this(
            data.Position + offset,
            GetSide(data.Enum("side", Sides.Up)),
            data.Bool("persistent", false),
            new EntityID(data.Level.Name, data.ID),
            data.Bool("refillDashOnCollision", false),
            data.Bool("doubleDashRefill", false),
            data.Bool("isWindTrigger", false),
            data.Enum("windPatternOnCollision", WindController.Patterns.None)) {
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);

            if (refillDash && !baseData.Get<bool>("pressed")) {
                Sides side = baseData.Get<Sides>("side");
                Vector2 bloomPosition = side switch {
                    Sides.Up => new Vector2(8f, 8f),
                    Sides.Down => new Vector2(8f, 0f),
                    Sides.Left => new Vector2(8f, 8f),
                    Sides.Right => new Vector2(0f, 8f),
                    _ => Vector2.Zero
                };

                Add(bloom = new BloomPoint(bloomPosition, 0.7f, 24f));
            }
        }

        public override void Update() {
            base.Update();

            if (refillDash && !baseData.Get<bool>("pressed") && Scene.OnInterval(0.1f)) {
                SceneAs<Level>().ParticlesFG.Emit(doubleDashRefill ? Refill.P_GlowTwo : Refill.P_Glow, 4, Position + bloom.Position, Vector2.One * 8f);
            }
        }

        internal static void Load() {
            On.Celeste.DashSwitch.OnDashed += DashSwitch_OnDashed;
        }

        internal static void Unload() {
            On.Celeste.DashSwitch.OnDashed -= DashSwitch_OnDashed;
        }

        // We can't just set a new collision delegate because DashSwitch also calls the method manually lol
        private static DashCollisionResults DashSwitch_OnDashed(On.Celeste.DashSwitch.orig_OnDashed orig, DashSwitch self, Player player, Vector2 direction) {
            if (self is ShroomDashSwitch sw) {
                bool pressedBefore = DynamicData.For(self).Get<bool>("pressed");
                DashCollisionResults results = orig(self, player, direction);
                bool pressedAfter = DynamicData.For(self).Get<bool>("pressed");

                if (!pressedBefore && pressedAfter) {
                    sw.OnPressShroomSwitch(player);
                }

                return results;
            }

            return orig(self, player, direction);
        }

        // The side is the press direction, but the plugin interpreted it as the facing direction :)
        private static Sides GetSide(Sides side) {
            return side switch {
                Sides.Up => Sides.Down,
                Sides.Down => Sides.Up,
                Sides.Left => Sides.Right,
                Sides.Right => Sides.Left,
                _ => Sides.Up
            };
        }

        // DashSwitch prepends "dashSwitch_" to this sprite name to build the sprite ID
        private static string GetSpriteName(bool refill, bool doubleRefill) {
            if (refill) {
                if (doubleRefill) {
                    return "ShroomHelperDoubleRefill";
                }
                return "ShroomHelperRefill";
            }
            return "ShroomHelper";
        }

        private void OnPressShroomSwitch(Player player) {
            if (refillDash && player != null) {
                player.UseRefill(doubleDashRefill);
                Audio.Play(doubleDashRefill ? SFX.game_10_pinkdiamond_touch : SFX.game_gen_diamond_touch, Position);
                Remove(bloom);
            }

            if (isWindTrigger) {
                SetWind();
            }
        }

        private void SetWind() {
            if (Scene.Entities.FindFirst<WindController>() is WindController wc) {
                wc.SetPattern(windPattern);
            } else {
                Scene.Add(new WindController(windPattern));
            }
        }
    }
}
