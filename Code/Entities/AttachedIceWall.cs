using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.ShroomHelper.Entities {
    [CustomEntity("ShroomHelper/AttachedIceWall")]
    [Tracked(false)]
    public class AttachedIceWall : Entity {
        public Facings Facing;

        private readonly int spriteOffset;
        private readonly StaticMover staticMover;
        private readonly ClimbBlocker climbBlocker;
        private readonly List<Sprite> tiles;
        private Vector2 imageOffset;

        public AttachedIceWall(Vector2 position, float height, bool left, int offset)
            : base(position) {
            spriteOffset = offset;
            Tag = Tags.TransitionUpdate;
            Depth = 1999;
            staticMover = new StaticMover {
                OnShake = OnShake,
                OnAttach = delegate (Platform p) {
                    Depth = p.Depth + 1;
                }
            };

            if (left) {
                Facing = Facings.Left;
                Collider = new Hitbox(2f, height);
                staticMover.SolidChecker = (Solid s) => CollideCheck(s, Position - Vector2.UnitX);
                Add(staticMover);
            } else {
                Facing = Facings.Right;
                Collider = new Hitbox(2f, height, 6f);
                staticMover.SolidChecker = (Solid s) => CollideCheck(s, Position + Vector2.UnitX);
                Add(staticMover);
            }

            Add(climbBlocker = new ClimbBlocker(edge: false));
            tiles = BuildSprite(left);

            staticMover.OnEnable = OnEnable;
            staticMover.OnDisable = OnDisable;
        }

        public AttachedIceWall(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Height, data.Bool("left", true), data.Int("spriteOffset", 0)) {
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            SetIceWalls();
        }

        public override void Update() {
            if (!SceneAs<Level>().Transitioning) {
                base.Update();
            }
        }

        public override void Render() {
            Vector2 position = Position;
            Position += imageOffset;
            base.Render();
            Position = position;
        }

        private void OnEnable() {
            Visible = Collidable = true;
        }

        private void OnDisable() {
            Visible = Collidable = false;
        }

        private void OnShake(Vector2 amount) {
            imageOffset += amount;
        }

        private List<Sprite> BuildSprite(bool left) {
            List<Sprite> list = new();
            for (int i = 0; i < Height; i += 8) {
                string id = (i == 0) ? "WallBoosterTop" : ((!(i + 16 > Height)) ? "WallBoosterMid" : "WallBoosterBottom");
                Sprite sprite = GFX.SpriteBank.Create(id);
                if (!left) {
                    sprite.FlipX = true;
                    sprite.Position = new Vector2(4f + spriteOffset, i);
                } else {
                    sprite.Position = new Vector2(0f - spriteOffset, i);
                }

                list.Add(sprite);
                Add(sprite);
            }

            return list;
        }

        private void SetIceWalls() {
            climbBlocker.Blocking = true;
            tiles.ForEach(delegate (Sprite t) {
                t.Play("ice");
            });
        }
    }
}
