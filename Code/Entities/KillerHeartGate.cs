using System;
using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.ShroomHelper.Entities
{
    [CustomEntity("ShroomHelper/KillerHeartGate")]
    class KillerHeartGate : Entity
    {

        private struct Particle
        {
            public Vector2 Position;

            public float Speed;

            public Color Color;
        }

        private class WhiteLine : Entity
        {
            private float fade = 1f;

            private int blockSize;

            public WhiteLine(Vector2 origin, int blockSize)
                : base(origin)
            {
                base.Depth = -1000000;
                this.blockSize = blockSize;
            }

            public override void Update()
            {
                base.Update();
                fade = Calc.Approach(fade, 0f, Engine.DeltaTime);
                if (!(fade <= 0f))
                {
                    return;
                }
                RemoveSelf();
                Level level = SceneAs<Level>();
                for (float num = (int)level.Camera.Left; num < level.Camera.Right; num += 1f)
                {
                    if (num < base.X || num >= base.X + (float)blockSize)
                    {
                        level.Particles.Emit(P_Slice, new Vector2(num, base.Y));
                    }
                }
            }

            public override void Render()
            {
                Vector2 position = (base.Scene as Level).Camera.Position;
                float num = Math.Max(1f, 4f * fade);
                Draw.Rect(position.X - 10f, base.Y - num / 2f, 340f, num, Color.White);
            }
        }

        private const string OpenedFlag = "opened_heartgem_door_";

        public static ParticleType P_Shimmer;

        public static ParticleType P_Slice;

        public readonly int Requires = 1000;

        public int Size;

        public string customColor = "ffffff";

        public float triggerDistance = 100f;

        private readonly float openDistance;

        private float openPercent;

        private Solid TopSolid;

        private Solid BotSolid;

        private float offset;

        private Vector2 mist;

        private MTexture temp = new MTexture();

        private List<MTexture> icon;

        private Particle[] particles = new Particle[50];

        private bool startHidden = true;

        private float heartAlpha = 1f;

        public int HeartGems
        {
            get
            {
                if (SaveData.Instance.CheatMode)
                {
                    return Requires;
                }
                return SaveData.Instance.TotalHeartGems;
            }
        }

        public float Counter
        {
            get;
            private set;
        }

        public bool Opened
        {
            get;
            private set;
        }

        private float openAmount => openPercent * openDistance;

        public KillerHeartGate(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            Requires = 1000;
            Add(new CustomBloom(RenderBloom));
            Size = data.Width;
            customColor = data.Attr("customColor");
            triggerDistance = data.Float("triggerDistance");
            openDistance = 32f;
            Vector2? vector = data.FirstNodeNullable(offset);
            if (vector.HasValue)
            {
                openDistance = Math.Abs(vector.Value.Y - base.Y);
            }
            icon = GFX.Game.GetAtlasSubtextures("objects/heartdoor/icon");
            startHidden = true;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level level = scene as Level;
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Position = new Vector2(Calc.Random.NextFloat(Size), Calc.Random.NextFloat(level.Bounds.Height));
                particles[i].Speed = Calc.Random.Range(4, 12);
                particles[i].Color = Color.White * Calc.Random.Range(0.2f, 0.6f);
            }
            level.Add(TopSolid = new Solid(new Vector2(base.X, level.Bounds.Top - 32), Size, base.Y - (float)level.Bounds.Top + 32f, safe: true));
            TopSolid.SurfaceSoundIndex = 32;
            TopSolid.SquishEvenInAssistMode = true;
            TopSolid.EnableAssistModeChecks = false;
            level.Add(BotSolid = new Solid(new Vector2(base.X, base.Y), Size, (float)level.Bounds.Bottom - base.Y + 32f, safe: true));
            BotSolid.SurfaceSoundIndex = 32;
            BotSolid.SquishEvenInAssistMode = true;
            BotSolid.EnableAssistModeChecks = false;
            if ((base.Scene as Level).Session.GetFlag("opened_heartgem_door_" + Requires))
            {
                Opened = true;
                Visible = true;
                openPercent = 1f;
                Counter = Requires;
                TopSolid.Y -= openDistance;
                BotSolid.Y += openDistance;
            }
            else
            {
                Add(new Coroutine(Routine()));
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (Opened)
            {
                base.Scene.CollideFirst<DashBlock>(BotSolid.Collider.Bounds)?.RemoveSelf();
            }
            else if (startHidden)
            {
                Visible = false;
            }
        }

        private IEnumerator Routine()
        {
            Level level = Scene as Level;
            if (startHidden)
            {
                Player player;
                do
                {
                    yield return null;
                    player = Scene.Tracker.GetEntity<Player>();
                }
                while (player == null || !(Math.Abs(player.X - Center.X) < triggerDistance));
                Audio.Play("event:/new_content/game/10_farewell/heart_door", Position);
                Visible = true;
                heartAlpha = 0f;
                float topTo = TopSolid.Y;
                float botTo = BotSolid.Y;
                float topFrom = TopSolid.Y -= 240f;
                float botFrom = BotSolid.Y -= 240f;
                for (float p = 0f; p < 1f; p += Engine.DeltaTime * 1.2f)
                {
                    float ease = Ease.CubeIn(p);
                    TopSolid.MoveToY(topFrom + (topTo - topFrom) * ease);
                    BotSolid.MoveToY(botFrom + (botTo - botFrom) * ease);
                    DashBlock block = Scene.CollideFirst<DashBlock>(BotSolid.Collider.Bounds);
                    if (block != null)
                    {
                        block.Break(BotSolid.BottomCenter, new Vector2(0f, 1f), playSound: true, playDebrisSound: false);
                        Player player3 = Scene.Tracker.GetEntity<Player>();
                        if (player3 != null && Math.Abs(player3.X - Center.X) < 40f)
                        {
                            player3.PointBounce(player3.Position + Vector2.UnitX * 8f);
                        }
                    }
                    yield return null;
                }
                TopSolid.Y = topTo;
                BotSolid.Y = botTo;
                while (heartAlpha < 1f)
                {
                    heartAlpha = Calc.Approach(heartAlpha, 1f, Engine.DeltaTime * 2f);
                    yield return null;
                }
                yield return 0.06f;
            }
        }

        public override void Update()
        {
            base.Update();
            if (!Opened)
            {
                offset += 12f * Engine.DeltaTime;
                mist.X -= 4f * Engine.DeltaTime;
                mist.Y -= 24f * Engine.DeltaTime;
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].Position.Y += particles[i].Speed * Engine.DeltaTime;
                }
            }
        }

        public void RenderBloom()
        {
            if (!Opened && Visible)
            {
                DrawBloom(new Rectangle((int)TopSolid.X, (int)TopSolid.Y, Size, (int)(TopSolid.Height + BotSolid.Height)));
            }
        }

        private void DrawBloom(Rectangle bounds)
        {
            Draw.Rect(bounds.Left - 4, bounds.Top, 2f, bounds.Height, Color.White * 0.25f);
            Draw.Rect(bounds.Left - 2, bounds.Top, 2f, bounds.Height, Color.White * 0.5f);
            Draw.Rect(bounds, Color.White * 0.75f);
            Draw.Rect(bounds.Right, bounds.Top, 2f, bounds.Height, Color.White * 0.5f);
            Draw.Rect(bounds.Right + 2, bounds.Top, 2f, bounds.Height, Color.White * 0.25f);
        }

        private void DrawMist(Rectangle bounds, Vector2 mist)
        {
            Color color = Color.White * 0.6f;
            MTexture mTexture = GFX.Game["objects/heartdoor/mist"];
            int num = mTexture.Width / 2;
            int num2 = mTexture.Height / 2;
            for (int i = 0; i < bounds.Width; i += num)
            {
                for (int j = 0; j < bounds.Height; j += num2)
                {
                    mTexture.GetSubtexture((int)Mod(mist.X, num), (int)Mod(mist.Y, num2), Math.Min(num, bounds.Width - i), Math.Min(num2, bounds.Height - j), temp);
                    temp.Draw(new Vector2(bounds.X + i, bounds.Y + j), Vector2.Zero, color);
                }
            }
        }

        private void DrawInterior(Rectangle bounds)
        {
            Draw.Rect(bounds, Calc.HexToColor(customColor));
            DrawMist(bounds, mist);
            DrawMist(bounds, new Vector2(mist.Y, mist.X) * 1.5f);
            Vector2 value = (base.Scene as Level).Camera.Position;
            if (Opened)
            {
                value = Vector2.Zero;
            }
            for (int i = 0; i < particles.Length; i++)
            {
                Vector2 value2 = particles[i].Position + value * 0.2f;
                value2.X = Mod(value2.X, bounds.Width);
                value2.Y = Mod(value2.Y, bounds.Height);
                Draw.Pixel.Draw(new Vector2(bounds.X, bounds.Y) + value2, Vector2.Zero, particles[i].Color);
            }
        }

        private void DrawEdges(Rectangle bounds, Color color)
        {
            MTexture mTexture = GFX.Game["objects/heartdoor/edge"];
            MTexture mTexture2 = GFX.Game["objects/heartdoor/top"];
            int num = (int)(offset % 8f);
            if (num > 0)
            {
                mTexture.GetSubtexture(0, 8 - num, 7, num, temp);
                temp.DrawJustified(new Vector2(bounds.Left + 4, bounds.Top), new Vector2(0.5f, 0f), color, new Vector2(-1f, 1f));
                temp.DrawJustified(new Vector2(bounds.Right - 4, bounds.Top), new Vector2(0.5f, 0f), color, new Vector2(1f, 1f));
            }
            for (int i = num; i < bounds.Height; i += 8)
            {
                mTexture.GetSubtexture(0, 0, 8, Math.Min(8, bounds.Height - i), temp);
                temp.DrawJustified(new Vector2(bounds.Left + 4, bounds.Top + i), new Vector2(0.5f, 0f), color, new Vector2(-1f, 1f));
                temp.DrawJustified(new Vector2(bounds.Right - 4, bounds.Top + i), new Vector2(0.5f, 0f), color, new Vector2(1f, 1f));
            }
            for (int j = 0; j < bounds.Width; j += 8)
            {
                mTexture2.DrawCentered(new Vector2(bounds.Left + 4 + j, bounds.Top + 4), color);
                mTexture2.DrawCentered(new Vector2(bounds.Left + 4 + j, bounds.Bottom - 4), color, new Vector2(1f, -1f));
            }
        }

        public override void Render()
        {
            Color color = Opened ? (Color.White * 0.25f) : Color.White;
            if (!Opened && TopSolid.Visible && BotSolid.Visible)
            {
                Rectangle bounds = new Rectangle((int)TopSolid.X, (int)TopSolid.Y, Size, (int)(TopSolid.Height + BotSolid.Height));
                DrawInterior(bounds);
                DrawEdges(bounds, color);
            }
            else
            {
                if (TopSolid.Visible)
                {
                    Rectangle bounds2 = new Rectangle((int)TopSolid.X, (int)TopSolid.Y, Size, (int)TopSolid.Height);
                    DrawInterior(bounds2);
                    DrawEdges(bounds2, color);
                }
                if (BotSolid.Visible)
                {
                    Rectangle bounds3 = new Rectangle((int)BotSolid.X, (int)BotSolid.Y, Size, (int)BotSolid.Height);
                    DrawInterior(bounds3);
                    DrawEdges(bounds3, color);
                }
            }
        }

        private float Mod(float x, float m)
        {
            return (x % m + m) % m;
        }
    }
}

