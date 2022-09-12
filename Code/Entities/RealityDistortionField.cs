using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.ShroomHelper.Entities {
    [CustomEntity("ShroomHelper/RealityDistortionField")]
    [Tracked(false)]
    public class RealityDistortionField : Entity {
        public float rippleAreaMultiplier;
        public uint Seed;
        public uint Seed2;
        public float floatingOpacity = 0.01f;

        private readonly bool active = true;
        private bool glitcherAdded;

        public RealityDistortionField(EntityData data, Vector2 offset) 
            : base(data.Position + offset) {
            rippleAreaMultiplier = data.Float("rippleAreaMultiplier", 1.0f);
            Depth = 2000;
        }

        public override void Update() {
            if (!glitcherAdded) {
                Add(new Coroutine(GlitchEffect()));
            }

            base.Update();
        }

        private IEnumerator GlitchEffect() {
            glitcherAdded = true;
            Level level = Scene as Level;
            float width = Width;
            float height = Height;
            while (active) {
                level.Displacement.AddBurst(Center, 10f * (rippleAreaMultiplier / 6.0f), 0f, width + (10f * rippleAreaMultiplier), 0.8f);
                yield return 5;
            }

            glitcherAdded = false;
            yield return true;
        }
    }
}
