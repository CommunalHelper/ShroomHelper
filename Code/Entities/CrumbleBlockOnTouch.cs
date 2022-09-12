using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.ShroomHelper.Entities {
    [CustomEntity("ShroomHelper/CrumbleBlockOnTouch")]
    [Tracked(false)]
    public class CrumbleBlockOnTouch : Solid {
        public bool permanent;
        public float delay;
        public bool triggered;
        public bool blendIn;

        private readonly char tileType;
        private EntityID id;

        public CrumbleBlockOnTouch(Vector2 position, char tileType, float width, float height, bool blendIn, bool persistent, float delay, EntityID id)
            : base(position, width, height, safe: true) {
            Depth = -12999;
            this.id = id;
            this.tileType = tileType;
            this.blendIn = blendIn;
            this.delay = delay;
            permanent = persistent;
            SurfaceSoundIndex = SurfaceIndex.TileToIndex[this.tileType];
        }

        public CrumbleBlockOnTouch(EntityData data, Vector2 offset, EntityID id)
            : this(data.Position + offset, data.Char("tiletype", 'm'), data.Width, data.Height, data.Bool("blendin"), data.Bool("persistent"), data.Float("delay"), id) {
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            TileGrid tileGrid;
            if (!blendIn) {
                tileGrid = GFX.FGAutotiler.GenerateBox(tileType, (int)Width / 8, (int)Height / 8).TileGrid;
            } else {
                Level level = SceneAs<Level>();
                Rectangle tileBounds = level.Session.MapData.TileBounds;
                VirtualMap<char> solidsData = level.SolidsData;
                int x = (int)(X / 8f) - tileBounds.Left;
                int y = (int)(Y / 8f) - tileBounds.Top;
                int tilesX = (int)Width / 8;
                int tilesY = (int)Height / 8;
                tileGrid = GFX.FGAutotiler.GenerateOverlay(tileType, x, y, tilesX, tilesY, solidsData).TileGrid;
                Depth = -10501;
            }

            Add(tileGrid);
            Add(new Coroutine(Sequence()));
            Add(new TileInterceptor(tileGrid, highPriority: true));
            Add(new LightOcclude());
            if (CollideCheck<Player>()) {
                RemoveSelf();
            }
        }

        public override void OnStaticMoverTrigger(StaticMover sm) {
            triggered = true;
        }

        public void Break() {
            if (!Collidable || Scene == null) {
                return;
            }

            Audio.Play("event:/new_content/game/10_farewell/quake_rockbreak", Position);
            Collidable = false;
            for (int i = 0; i < Width / 8f; i++) {
                for (int j = 0; j < Height / 8f; j++) {
                    if (!Scene.CollideCheck<Solid>(new Rectangle((int)X + (i * 8), (int)Y + (j * 8), 8, 8))) {
                        Scene.Add(Engine.Pooler.Create<Debris>().Init(Position + new Vector2(4 + (i * 8), 4 + (j * 8)), tileType, playSound: true).BlastFrom(TopCenter));
                    }
                }
            }

            if (permanent) {
                Level level = SceneAs<Level>();
                level.Session.DoNotLoad.Add(id);
            }

            RemoveSelf();
        }

        public bool HasPlayerLeaning() {
            Player player = Scene.Tracker.GetEntity<Player>();
            if (player.Facing == Facings.Left && CollideCheck(player, Position + Vector2.UnitX)) {
                return true;
            } else if (player.Facing == Facings.Right && CollideCheck(player, Position - Vector2.UnitX)) {
                return true;
            } else {
                return false;
            }
        }

        private bool PlayerBreakCheck() {
            return HasPlayerRider() || HasPlayerLeaning();
        }

        private IEnumerator Sequence() {
            while (!triggered && !PlayerBreakCheck()) {
                yield return null;
            }

            while (delay > 0f) {
                delay -= Engine.DeltaTime;
                yield return null;
            }

            while (true) {
                Break();
                yield break;
            }
        }
    }
}
