using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.ShroomHelper.Entities
{
    [CustomEntity("ShroomHelper/RealityDistortionField")]
    [Tracked(false)]
    public class RealityDistortionField : Entity
    {
        public float rippleAreaMultiplier;

        public uint Seed;
        public uint Seed2;

        private bool glitcherAdded;
        private bool active = true;

        public float floatingOpacity = 0.01f;

        public RealityDistortionField(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            rippleAreaMultiplier = data.Float("rippleAreaMultiplier", 1.0f);
            base.Depth = 2000;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
        }

        public override void Update()
        {
            if (!glitcherAdded)
            {
                Add(new Coroutine(glitchEffect()));
            }
            base.Update();
        }

        private IEnumerator glitchEffect()
        {
            glitcherAdded = true;
            Level level = base.Scene as Level;
            float width = base.Width;
            float height = base.Height;
            while (active)
            {
                level.Displacement.AddBurst(base.Center, 10f * (rippleAreaMultiplier / 6.0f), 0f, width + 10f * rippleAreaMultiplier, 0.8f);
                yield return 5;
            }
            glitcherAdded = false;
            yield return true;
        }
    }
}
