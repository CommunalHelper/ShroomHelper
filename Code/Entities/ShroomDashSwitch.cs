using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.ShroomHelper.Entities {
    [CustomEntity("ShroomHelper/ShroomDashSwitch")]
    public class ShroomDashSwitch : Solid {
        private readonly Sides side;
        private readonly float startY;
        private readonly bool persistent;
        private readonly bool refillDashOnCollision;
        private readonly bool doubleDashRefill;
        private readonly bool isWindTrigger;
        private readonly WindController.Patterns windPatternOnCollision;
        private readonly bool mirrorMode;
        private readonly bool allGates;
        private readonly Sprite sprite;

        private Vector2 pressedTarget;
        private bool pressed;
        private Vector2 pressDirection;
        private float speedY;
        private EntityID id;
        private bool playerWasOn;
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
            ) : base(position, 0f, 0f, safe: true) {
            this.side = side;
            this.persistent = persistent;
            this.allGates = allGates;
            this.id = id;
            this.refillDashOnCollision = refillDashOnCollision;
            this.doubleDashRefill = doubleDashRefill;
            this.windPatternOnCollision = windPatternOnCollision;
            this.isWindTrigger = isWindTrigger;

            mirrorMode = true;

            if (refillDashOnCollision) {
                if (doubleDashRefill) {
                    Add(sprite = ShroomHelperModule.spriteBank.Create("ShroomHelperDashSwitchDoubleRefill"));
                } else {
                    Add(sprite = ShroomHelperModule.spriteBank.Create("ShroomHelperDashSwitchRefill"));
                }
            } else {
                Add(sprite = ShroomHelperModule.spriteBank.Create("ShroomHelperDashSwitch"));
            }

            sprite.Play("idle");
            if (side is Sides.Up or Sides.Down) {
                Collider.Width = 16f;
                Collider.Height = 8f;
            } else {
                Collider.Width = 8f;
                Collider.Height = 16f;
            }

            switch (side) {
                case Sides.Up:
                    sprite.Position = new Vector2(8f, 8f);
                    sprite.Rotation = (float)Math.PI / 2f;
                    pressedTarget = Position + (Vector2.UnitY * 8f);
                    pressDirection = Vector2.UnitY;
                    bloomPoint = new Vector2(8f, 0f);
                    startY = Y;
                    break;
                case Sides.Down:
                    sprite.Position = new Vector2(8f, 0f);
                    sprite.Rotation = -(float)Math.PI / 2f;
                    pressedTarget = Position + (Vector2.UnitY * -8f);
                    pressDirection = -Vector2.UnitY;
                    bloomPoint = new Vector2(8f, 8f);
                    break;
                case Sides.Left:
                    sprite.Position = new Vector2(8f, 8f);
                    sprite.Rotation = 0f;
                    pressedTarget = Position + (Vector2.UnitX * 8f);
                    pressDirection = Vector2.UnitX;
                    bloomPoint = new Vector2(0f, 8f);
                    break;
                case Sides.Right:
                    sprite.Position = new Vector2(0f, 8f);
                    sprite.Rotation = (float)Math.PI;
                    pressedTarget = Position + (Vector2.UnitX * -8f);
                    pressDirection = -Vector2.UnitX;
                    bloomPoint = new Vector2(8f, 8f);
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
                  data.Bool("isWindTrigger", false)) {
        }

        public enum Sides {
            Up,
            Down,
            Left,
            Right
        }

        private string FlagName => GetFlagName(id);

        public static string GetFlagName(EntityID id) {
            return "dashSwitch_" + id.Key;
        }

        public static ShroomDashSwitch Create(EntityData data, Vector2 offset, EntityID id) {
            Vector2 position = data.Position + offset;
            bool flag = data.Bool("persistent", false);
            bool flag2 = data.Bool("allGates", false);
            bool refillDashOnCollision = data.Bool("refillDashOnCollision", false);
            bool doubleDashRefill = data.Bool("doubleDashRefill", false);
            string windPatternOnCollisionString = data.Attr("windPatternOnCollision", "None");
            WindController.Patterns windPatternOnCollision = (WindController.Patterns)Enum.Parse(typeof(WindController.Patterns), windPatternOnCollisionString);
            bool isWindTrigger = data.Bool("isWindTrigger", false);

            string spriteName = data.Attr("sprite", "default");
            if (data.Name.Equals("dashSwitchH")) {
                if (data.Bool("leftSide")) {
                    return new ShroomDashSwitch(position, Sides.Left, flag, flag2, id, spriteName, refillDashOnCollision, doubleDashRefill, windPatternOnCollision, isWindTrigger);
                }
                return new ShroomDashSwitch(position, Sides.Right, flag, flag2, id, spriteName, refillDashOnCollision, doubleDashRefill, windPatternOnCollision, isWindTrigger);
            }
            if (data.Bool("ceiling")) {
                return new ShroomDashSwitch(position, Sides.Down, flag, flag2, id, spriteName, refillDashOnCollision, doubleDashRefill, windPatternOnCollision, isWindTrigger);
            }
            return new ShroomDashSwitch(position, Sides.Up, flag, flag2, id, spriteName, refillDashOnCollision, doubleDashRefill, windPatternOnCollision, isWindTrigger);
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);

            if (refillDashOnCollision) {
                Add(bloom = new BloomPoint(bloomPoint, 0.7f, 24f));
            }

            if (persistent && SceneAs<Level>().Session.GetFlag(FlagName)) {
                sprite.Play("pushed");
                Position = pressedTarget - (pressDirection * 2f);
                pressed = true;
                Collidable = false;
                if (allGates) {
                    foreach (TempleGate gate in Scene.Tracker.GetEntities<TempleGate>()) {
                        if (gate.Type == TempleGate.Types.NearestSwitch && gate.LevelID == id.Level) {
                            gate.StartOpen();
                        }
                    }
                } else {
                    GetGate()?.StartOpen();
                }
            }
        }

        public override void Update() {
            base.Update();

            // sparkles if not pressed
            if (!pressed && refillDashOnCollision) {
                if (Scene.OnInterval(0.1f)) {
                    SceneAs<Level>().ParticlesFG.Emit(doubleDashRefill ? Refill.P_GlowTwo : Refill.P_Glow, 4, Position + bloomPoint, Vector2.One * 8f);
                }
            }

            if (pressed || side != Sides.Up) {
                return;
            }

            Player playerOnTop = GetPlayerOnTop();
            if (playerOnTop != null) {
                if (playerOnTop.Holding != null) {
                    OnDashed(playerOnTop, Vector2.UnitY);
                } else {
                    if (speedY < 0f) {
                        speedY = 0f;
                    }

                    speedY = Calc.Approach(speedY, 70f, 200f * Engine.DeltaTime);
                    MoveTowardsY(startY + 2f, speedY * Engine.DeltaTime);
                    if (!playerWasOn) {
                        Audio.Play("event:/game/05_mirror_temple/button_depress", Position);
                    }
                }
            } else {
                if (speedY > 0f) {
                    speedY = 0f;
                }

                speedY = Calc.Approach(speedY, -150f, 200f * Engine.DeltaTime);
                MoveTowardsY(startY, (0f - speedY) * Engine.DeltaTime);
                if (playerWasOn) {
                    Audio.Play("event:/game/05_mirror_temple/button_return", Position);
                }
            }

            playerWasOn = playerOnTop != null;
        }

        public DashCollisionResults OnDashed(Player player, Vector2 direction) {
            if (!pressed && direction == pressDirection) {
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
                if (refillDashOnCollision) {
                    player.UseRefill(doubleDashRefill);
                    Audio.Play(doubleDashRefill ? "event:/new_content/game/10_farewell/pinkdiamond_touch" : "event:/game/general/diamond_touch", Position);
                    Remove(bloom);
                }

                // wind pattern
                if (isWindTrigger) {
                    SetWind();
                }

                if (allGates) {
                    foreach (TempleGate gate in Scene.Tracker.GetEntities<TempleGate>()) {
                        if (gate.Type == TempleGate.Types.NearestSwitch && gate.LevelID == id.Level) {
                            gate.SwitchOpen();
                        }
                    }
                } else {
                    GetGate()?.SwitchOpen();
                }

                Scene.Entities.FindFirst<TempleMirrorPortal>()?.OnSwitchHit(Math.Sign(X - SceneAs<Level>().Bounds.Center.X));
                if (persistent) {
                    SceneAs<Level>().Session.SetFlag(FlagName);
                }
            }

            return DashCollisionResults.NormalCollision;
        }

        public void SetWind() {
            WindController windController = Scene.Entities.FindFirst<WindController>();
            if (windController == null) {
                windController = new WindController(windPatternOnCollision);
                Scene.Add(windController);
            } else {
                windController.SetPattern(windPatternOnCollision);
            }
        }

        private TempleGate GetGate() {
            List<Entity> entities = Scene.Tracker.GetEntities<TempleGate>();
            TempleGate templeGate = null;
            float num = 0f;
            foreach (TempleGate item in entities) {
                if (item.Type == TempleGate.Types.NearestSwitch && !item.ClaimedByASwitch && item.LevelID == id.Level) {
                    float num2 = Vector2.DistanceSquared(Position, item.Position);
                    if (templeGate == null || num2 < num) {
                        templeGate = item;
                        num = num2;
                    }
                }
            }

            if (templeGate != null) {
                templeGate.ClaimedByASwitch = true;
            }

            return templeGate;
        }
    }
}
