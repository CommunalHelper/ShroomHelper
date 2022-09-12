using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.ShroomHelper.Entities {
    [CustomEntity("ShroomHelper/DoubleRefillBooster")]
    public class DoubleRefillBooster : Booster {
        protected DynamicData baseData;

        public DoubleRefillBooster(Vector2 position, bool red)
            : base(position, red) {
            baseData = new DynamicData(typeof(Booster), this);
        }

        public DoubleRefillBooster(EntityData data, Vector2 offset)
            : this(data.Position + offset, true) {
        }

        public static void Load() {
            On.Celeste.Player.RedBoost += Player_RedBoost;
        }

        public static void Unload() {
            On.Celeste.Player.RedBoost -= Player_RedBoost;
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            Sprite sprite = baseData.Get<Sprite>("sprite");
            Remove(sprite);
            sprite = ShroomHelperModule.spriteBank.Create("ShroomHelperDoubleRefillBooster");
            sprite.Play("loop");
            baseData.Set("sprite", sprite);
            Add(sprite);
            Add(new BloomPoint(0.8f, 24f));
        }

        public override void Update() {
            base.Update();
            if (Scene.OnInterval(0.1f)) {
                SceneAs<Level>().ParticlesFG.Emit(Refill.P_GlowTwo, 4, Position, Vector2.One * 12f);
            }
        }

        private static void Player_RedBoost(On.Celeste.Player.orig_RedBoost orig, Player self, Booster booster) {
            orig(self, booster);
            if (booster is DoubleRefillBooster) {
                self.UseRefill(true);
                Audio.Play("event:/new_content/game/10_farewell/pinkdiamond_touch", booster.Position);
            }
        }
    }
}
