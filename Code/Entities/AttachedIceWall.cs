using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.ShroomHelper.Entities
{
    [CustomEntity("ShroomHelper/AttachedIceWall")]
    [Tracked(false)]
    class AttachedIceWall : Entity
    {
        public new Facings Facing;

        private int spriteOffset;
 
        private StaticMover staticMover;

        private ClimbBlocker climbBlocker;

        private Vector2 imageOffset;

        private List<Sprite> tiles;

        public AttachedIceWall(Vector2 position, float height, bool left, int offset)
            : base(position)
        {
            spriteOffset = offset;
            base.Tag = Tags.TransitionUpdate;
            base.Depth = 1999;
            staticMover = new StaticMover();
            staticMover.OnShake = OnShake;
            staticMover.OnAttach = delegate (Platform p)
            {
                base.Depth = p.Depth + 1;
            };

            if (left)
            {
                Facing = Facings.Left;
                base.Collider = new Hitbox(2f, height);
                staticMover.SolidChecker = ((Solid s) => CollideCheck(s, Position - Vector2.UnitX));
                Add(staticMover);
            }
            else
            {
                Facing = Facings.Right;
                base.Collider = new Hitbox(2f, height, 6f);
                staticMover.SolidChecker = ((Solid s) => CollideCheck(s, Position + Vector2.UnitX));
                Add(staticMover);
            }
            Add(climbBlocker = new ClimbBlocker(edge: false));
            tiles = BuildSprite(left);

            staticMover.OnEnable = OnEnable;
            staticMover.OnDisable = OnDisable;
        }

        private void OnEnable()
        {
            Visible = (Collidable = true);
        }

        private void OnDisable()
        {
            Collidable = false;
            Visible = false;
        }

        private void OnShake(Vector2 amount)
        {
            imageOffset += amount;
        }

        public AttachedIceWall(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Height, data.Bool("left"), data.Int("spriteOffset", 1))
        {
        }

        public override void Render()
        {
            Vector2 position = Position;
            Position += imageOffset;
            base.Render();
            Position = position;
        }

        private List<Sprite> BuildSprite(bool left)
        {
            List<Sprite> list = new List<Sprite>();
            for (int i = 0; (float)i < base.Height; i += 8)
            {
                string id = (i == 0) ? "WallBoosterTop" : ((!((float)(i + 16) > base.Height)) ? "WallBoosterMid" : "WallBoosterBottom");
                Sprite sprite = GFX.SpriteBank.Create(id);
                if (!left)
                {
                    sprite.FlipX = true;
                    sprite.Position = new Vector2(4f + spriteOffset, i);
                }
                else
                {
                    sprite.Position = new Vector2(0f - spriteOffset, i);
                }
                list.Add(sprite);
                Add(sprite);
            }
            return list;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            SetIceWalls();
        }

        private void SetIceWalls()
        {
            climbBlocker.Blocking = true;
            tiles.ForEach(delegate (Sprite t)
            {
                t.Play("ice");
            });
        }

        public override void Update()
        {
            if (!(base.Scene as Level).Transitioning)
            {
                base.Update();
            }
        }
    }
}
