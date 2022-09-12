using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;

namespace Celeste.Mod.ShroomHelper.Entities
{
    [CustomEntity("ShroomHelper/ShroomDashSwitch")]
    class ShroomDashSwitch : Solid
    {
        public enum Sides
        {
            Up,
            Down,
            Left,
            Right
        }

        private Sides side;

        private Vector2 pressedTarget;

        private bool pressed;

        private Vector2 pressDirection;

        private float speedY;

        private float startY;

        private bool persistent;

        private bool refillDashOnCollision;

        private bool doubleDashRefill;

        private bool isWindTrigger;

        private WindController.Patterns windPatternOnCollision;

        private EntityID id;

        private bool mirrorMode;

        private bool playerWasOn;

        private bool allGates;

        private Sprite sprite;

        private string FlagName => GetFlagName(id);

        private BloomPoint bloom;

        private Vector2 bloomPoint;

        public ShroomDashSwitch(
                Vector2 position,
                Sides side,
                bool persistent,
                bool allGates,
                EntityID id,
                string spriteName,
                bool refillDashOnCollision,
                bool doubleDashRefill,
                WindController.Patterns windPatternOnCollision,
                bool isWindTrigger
            ) : base(position, 0f, 0f, safe: true)
        {
            this.side = side;
            this.persistent = persistent;
            this.allGates = allGates;
            this.id = id;
            this.refillDashOnCollision = refillDashOnCollision;
            this.doubleDashRefill = doubleDashRefill;
            this.windPatternOnCollision = windPatternOnCollision;
            this.isWindTrigger = isWindTrigger;

            mirrorMode = true;

            if (refillDashOnCollision)
            {
                if (doubleDashRefill)
                {
                    Add(sprite = ShroomHelperModule.spriteBank.Create("ShroomHelperDashSwitchDoubleRefill"));
                }
                else
                {
                    Add(sprite = ShroomHelperModule.spriteBank.Create("ShroomHelperDashSwitchRefill"));
                }
                
            }
            else
            {
                Add(sprite = ShroomHelperModule.spriteBank.Create("ShroomHelperDashSwitch"));
            }

            sprite.Play("idle");
            if (side == Sides.Up || side == Sides.Down)
            {
                base.Collider.Width = 16f;
                base.Collider.Height = 8f;
            }
            else
            {
                base.Collider.Width = 8f;
                base.Collider.Height = 16f;
            }
            switch (side)
            {
                case Sides.Up:
                    sprite.Position = new Vector2(8f, 8f);
                    sprite.Rotation = (float)Math.PI / 2f;
                    pressedTarget = Position + Vector2.UnitY * 8f;
                    pressDirection = Vector2.UnitY;
                    this.bloomPoint = new Vector2(8f, 0f);
                    startY = base.Y;
                    break;
                case Sides.Down:
                    sprite.Position = new Vector2(8f, 0f);
                    sprite.Rotation = -(float)Math.PI / 2f;
                    pressedTarget = Position + Vector2.UnitY * -8f;
                    pressDirection = -Vector2.UnitY;
                    this.bloomPoint = new Vector2(8f, 8f);
                    break;
                case Sides.Left:
                    sprite.Position = new Vector2(8f, 8f);
                    sprite.Rotation = 0f;
                    pressedTarget = Position + Vector2.UnitX * 8f;
                    pressDirection = Vector2.UnitX;
                    this.bloomPoint = new Vector2(0f, 8f);
                    break;
                case Sides.Right:
                    sprite.Position = new Vector2(0f, 8f);
                    sprite.Rotation = (float)Math.PI;
                    pressedTarget = Position + Vector2.UnitX * -8f;
                    pressDirection = -Vector2.UnitX;
                    this.bloomPoint = new Vector2(8f, 8f);
                    break;
            }
            OnDashCollide = OnDashed;
        }

        public ShroomDashSwitch(EntityData data, Vector2 offset)
        : this(
                  data.Position + offset,
                  data.Enum("side", Sides.Up),
                  data.Bool("persistent"),
                  false,
                  new EntityID(data.Level.Name, data.ID),
                  data.Attr("sprite", "default"),
                  data.Bool("refillDashOnCollision", false),
                  data.Bool("doubleDashRefill", false),
                  data.Enum("windPatternOnCollision", WindController.Patterns.None),
                  data.Bool("isWindTrigger", false)
             )
        {
        }

        public static ShroomDashSwitch Create(EntityData data, Vector2 offset, EntityID id)
        {
            Vector2 position = data.Position + offset;
            bool flag = data.Bool("persistent", false);
            bool flag2 = data.Bool("allGates", false);
            bool refillDashOnCollision = data.Bool("refillDashOnCollision", false);
            bool doubleDashRefill = data.Bool("doubleDashRefill", false);
            string windPatternOnCollisionString = data.Attr("windPatternOnCollision", "None");
            WindController.Patterns windPatternOnCollision = (WindController.Patterns)Enum.Parse(typeof(WindController.Patterns), windPatternOnCollisionString);
            bool isWindTrigger = data.Bool("isWindTrigger", false);

            string spriteName = data.Attr("sprite", "default");
            if (data.Name.Equals("dashSwitchH"))
            {
                if (data.Bool("leftSide"))
                {
                    return new ShroomDashSwitch(position, Sides.Left, flag, flag2, id, spriteName, refillDashOnCollision, doubleDashRefill, windPatternOnCollision, isWindTrigger);
                }
                return new ShroomDashSwitch(position, Sides.Right, flag, flag2, id, spriteName, refillDashOnCollision, doubleDashRefill, windPatternOnCollision, isWindTrigger);
            }
            if (data.Bool("ceiling"))
            {
                return new ShroomDashSwitch(position, Sides.Down, flag, flag2, id, spriteName, refillDashOnCollision, doubleDashRefill, windPatternOnCollision, isWindTrigger);
            }
            return new ShroomDashSwitch(position, Sides.Up, flag, flag2, id, spriteName, refillDashOnCollision, doubleDashRefill, windPatternOnCollision, isWindTrigger);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            if (refillDashOnCollision)
            {
                Add(bloom = new BloomPoint(bloomPoint, 0.7f, 24f));
            }


            if (persistent && SceneAs<Level>().Session.GetFlag(FlagName))
            {
                sprite.Play("pushed");
                Position = pressedTarget - pressDirection * 2f;
                pressed = true;
                Collidable = false;
                if (allGates)
                {
                    foreach (TempleGate entity in base.Scene.Tracker.GetEntities<TempleGate>())
                    {
                        if (entity.Type == TempleGate.Types.NearestSwitch && entity.LevelID == id.Level)
                        {
                            entity.StartOpen();
                        }
                    }
                }
                else
                {
                    GetGate()?.StartOpen();
                }
            }
        }

        public override void Update()
        {
            base.Update();

            // sparkles if not pressed
            if (!pressed && refillDashOnCollision)
            {
                if (base.Scene.OnInterval(0.1f))
                {
                    if (doubleDashRefill)
                    {
                        (base.Scene as Level).ParticlesFG.Emit(Refill.P_GlowTwo, 4, Position + bloomPoint, Vector2.One * 8f);
                    } else
                    {
                        (base.Scene as Level).ParticlesFG.Emit(Refill.P_Glow, 4, Position + bloomPoint, Vector2.One * 8f);
                    }
                    
                }
            }


            if (pressed || side != Sides.Up)
            {
                return;
            }
            Player playerOnTop = GetPlayerOnTop();
            if (playerOnTop != null)
            {
                if (playerOnTop.Holding != null)
                {
                    OnDashed(playerOnTop, Vector2.UnitY);
                }
                else
                {
                    if (speedY < 0f)
                    {
                        speedY = 0f;
                    }
                    speedY = Calc.Approach(speedY, 70f, 200f * Engine.DeltaTime);
                    MoveTowardsY(startY + 2f, speedY * Engine.DeltaTime);
                    if (!playerWasOn)
                    {
                        Audio.Play("event:/game/05_mirror_temple/button_depress", Position);
                    }
                }
            }
            else
            {
                if (speedY > 0f)
                {
                    speedY = 0f;
                }
                speedY = Calc.Approach(speedY, -150f, 200f * Engine.DeltaTime);
                MoveTowardsY(startY, (0f - speedY) * Engine.DeltaTime);
                if (playerWasOn)
                {
                    Audio.Play("event:/game/05_mirror_temple/button_return", Position);
                }
            }
            playerWasOn = (playerOnTop != null);
        }

        public DashCollisionResults OnDashed(Player player, Vector2 direction)
        {
            if (!pressed && direction == pressDirection)
            {
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                Audio.Play("event:/game/05_mirror_temple/button_activate", Position);
                sprite.Play("push");
                pressed = true;
                MoveTo(pressedTarget);
                Collidable = false;
                Position -= pressDirection * 2f;
                SceneAs<Level>().ParticlesFG.Emit(mirrorMode ? DashSwitch.P_PressAMirror : DashSwitch.P_PressA, 10, Position + sprite.Position, direction.Perpendicular() * 6f, sprite.Rotation - (float)Math.PI);
                SceneAs<Level>().ParticlesFG.Emit(mirrorMode ? DashSwitch.P_PressBMirror : DashSwitch.P_PressB, 4, Position + sprite.Position, direction.Perpendicular() * 6f, sprite.Rotation - (float)Math.PI);

                // refill dash on collision
                if (refillDashOnCollision)
                {
                    player.UseRefill(doubleDashRefill);
                    Audio.Play(doubleDashRefill ? "event:/new_content/game/10_farewell/pinkdiamond_touch" : "event:/game/general/diamond_touch", Position);
                    Remove(bloom);
                }


                // wind pattern
                if (isWindTrigger)
                {
                    SetWind();
                }

                if (allGates)
                {
                    foreach (TempleGate entity in base.Scene.Tracker.GetEntities<TempleGate>())
                    {
                        if (entity.Type == TempleGate.Types.NearestSwitch && entity.LevelID == id.Level)
                        {
                            entity.SwitchOpen();
                        }
                    }
                }
                else
                {
                    GetGate()?.SwitchOpen();
                }
                base.Scene.Entities.FindFirst<TempleMirrorPortal>()?.OnSwitchHit(Math.Sign(base.X - (float)(base.Scene as Level).Bounds.Center.X));
                if (persistent)
                {
                    SceneAs<Level>().Session.SetFlag(FlagName);
                }
            }
            return DashCollisionResults.NormalCollision;
        }

        private TempleGate GetGate()
        {
            List<Entity> entities = base.Scene.Tracker.GetEntities<TempleGate>();
            TempleGate templeGate = null;
            float num = 0f;
            foreach (TempleGate item in entities)
            {
                if (item.Type == TempleGate.Types.NearestSwitch && !item.ClaimedByASwitch && item.LevelID == id.Level)
                {
                    float num2 = Vector2.DistanceSquared(Position, item.Position);
                    if (templeGate == null || num2 < num)
                    {
                        templeGate = item;
                        num = num2;
                    }
                }
            }
            if (templeGate != null)
            {
                templeGate.ClaimedByASwitch = true;
            }
            return templeGate;
        }

        public static string GetFlagName(EntityID id)
        {
            return "dashSwitch_" + id.Key;
        }

        public void SetWind()
        {
            WindController windController = base.Scene.Entities.FindFirst<WindController>();
            // Logger.Log("wind pattern", windPatternOnCollision.ToString());
            if (windController == null)
            {
                windController = new WindController(windPatternOnCollision);
                base.Scene.Add(windController);
            }
            else
            {
                windController.SetPattern(windPatternOnCollision);
            }
        }
    }
}
