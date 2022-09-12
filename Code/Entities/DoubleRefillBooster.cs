using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.ShroomHelper.Entities
{
    [CustomEntity("ShroomHelper/DoubleRefillBooster")]
    public class DoubleRefillBooster : Booster
    {
        protected DynData<Booster> baseData;
        private BloomPoint bloom;

        public DoubleRefillBooster(Vector2 position, bool red)
            : base(position, red)
        {
            baseData = new DynData<Booster>(this);
        }

        public DoubleRefillBooster(EntityData data, Vector2 offset)
            : this(data.Position + offset, true)
        {
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Sprite sprite = baseData.Get<Sprite>("sprite");
            Remove(sprite);
            sprite = ShroomHelperModule.spriteBank.Create("ShroomHelperDoubleRefillBooster");
            sprite.Play("loop");
            baseData["sprite"] = sprite;
            Add(sprite);
            Add(bloom = new BloomPoint(0.8f, 24f));
        }

        public override void Update()
        {
            base.Update();
            if (base.Scene.OnInterval(0.1f))
            {
                (base.Scene as Level).ParticlesFG.Emit(Refill.P_GlowTwo, 4, Position, Vector2.One * 12f);
            }
        }

        public static void Load()
        {
            On.Celeste.Player.RedBoost += Player_RedBoost;
        }

        public static void Unload()
        {
            On.Celeste.Player.RedBoost -= Player_RedBoost;
        }

        static void Player_RedBoost(On.Celeste.Player.orig_RedBoost orig, Player self, Booster booster)
        {
            orig(self, booster);
            if (booster is DoubleRefillBooster)
            {
                self.UseRefill(true);
                Audio.Play("event:/new_content/game/10_farewell/pinkdiamond_touch", booster.Position);
            }
        }
    }
}
