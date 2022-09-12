using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.ShroomHelper.Effects
{
    [CustomEntity("ShroomHelper/ShroomPetals")]
    class ShroomPetals : Backdrop
    {
        public string defaultColor = "8B0000";

        private struct Particle
        {
            public Vector2 Position;

            public float Speed;

            public float Spin;

            public float MaxRotate;

            public float RotationCounter;
        }

        public string shroomColor = "8B0000";

        private Particle[] particles = new Particle[40];

        private float fade;

        public ShroomPetals()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                Reset(i);
            }
        }

        private void Reset(int i)
        {
            particles[i].Position = new Vector2(Calc.Random.Range(0, 352), Calc.Random.Range(0, 212));
            particles[i].Speed = Calc.Random.Range(6f, 16f);
            particles[i].Spin = Calc.Random.Range(8f, 12f) * 0.2f;
            particles[i].RotationCounter = Calc.Random.NextAngle();
            particles[i].MaxRotate = Calc.Random.Range(0.3f, 0.6f) * ((float)Math.PI / 2f);
        }

        public override void Update(Scene scene)
        {
            base.Update(scene);
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Position.Y += particles[i].Speed * Engine.DeltaTime;
                particles[i].RotationCounter += particles[i].Spin * Engine.DeltaTime;
            }
            fade = Calc.Approach(fade, Visible ? 1f : 0f, Engine.DeltaTime * 10);
        }

        public override void Render(Scene level)
        {
            if (!(fade <= 0f))
            {
                Camera camera = (level as Level).Camera;
                MTexture mTexture = GFX.Game["particles/petal"];
                for (int i = 0; i < particles.Length; i++)
                {
                    Vector2 position = default(Vector2);
                    position.X = -16f + Mod(particles[i].Position.X - camera.X, 352f);
                    position.Y = -16f + Mod(particles[i].Position.Y - camera.Y, 212f);
                    float num = (float)(1.5707963705062866 + Math.Sin(particles[i].RotationCounter * particles[i].MaxRotate) * 1.0);
                    position += Calc.AngleToVector(num, 4f);

                    mTexture.DrawCentered(position, Calc.HexToColor(shroomColor) * fade, 1f, num - 0.8f);
                }
            }
        }

        private float Mod(float x, float m)
        {
            return (x % m + m) % m;
        }
    }

}