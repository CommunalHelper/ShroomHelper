using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections;

namespace Celeste.Mod.ShroomHelper.Entities {
    public class ShroomBook : CutsceneEntity {
        private readonly Player player;
        private readonly string bookTextKey;
        private PoemPage poem;

        public ShroomBook(Player player, string bookTextKey) {
            this.player = player;
            this.bookTextKey = bookTextKey;
        }

        public override void OnBegin(Level level) {
            Add(new Coroutine(Routine()));
        }

        public override void OnEnd(Level level) {
            player.StateMachine.Locked = false;
            player.StateMachine.State = Player.StNormal;
            if (poem != null) {
                poem.RemoveSelf();
            }
        }

        private IEnumerator Routine() {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;
            poem = new PoemPage(bookTextKey);
            Scene.Add(poem);
            yield return poem.EaseIn();
            while (!Input.MenuConfirm.Pressed) {
                yield return null;
            }

            Audio.Play("event:/ui/main/button_lowkey");
            yield return poem.EaseOut();
            poem = null;
            EndCutscene(Level);
        }

        private class PoemPage : Entity {
            private readonly float scale = 1f;
            private readonly MTexture paper;
            private VirtualRenderTarget target;
            private float alpha = 1f;
            private float rotation = 0f;
            private float timer = 0f;
            private bool easingOut;

            public PoemPage(string bookTextKey) {
                Tag = Tags.HUD;
                paper = GFX.Gui[bookTextKey];
                Add(new BeforeRenderHook(BeforeRender));
            }

            public override void Removed(Scene scene) {
                if (target != null) {
                    target.Dispose();
                }

                target = null;
                base.Removed(scene);
            }

            public override void SceneEnd(Scene scene) {
                if (target != null) {
                    target.Dispose();
                }

                target = null;
                base.SceneEnd(scene);
            }

            public override void Update() {
                timer += Engine.DeltaTime;
                base.Update();
            }

            public override void Render() {
                if (target != null && (Scene is not Level level || (!level.FrozenOrPaused && level.RetryPlayerCorpse == null && !level.SkippingCutscene))) {
                    Draw.SpriteBatch.Draw((RenderTarget2D)target, Position, target.Bounds, Color.White * alpha, rotation, new Vector2(target.Width, target.Height) / 2f, scale, SpriteEffects.None, 0f);
                    if (!easingOut) {
                        GFX.Gui["textboxbutton"].DrawCentered(Position + new Vector2((target.Width / 2) + 40, (target.Height / 2) + ((timer % 1f < 0.25f) ? 6 : 0)));
                    }
                }
            }

            public void BeforeRender() {
                if (target == null) {
                    target = VirtualContent.CreateRenderTarget("journal-poem", paper.Width, paper.Height);
                }

                Engine.Graphics.GraphicsDevice.SetRenderTarget(target);
                Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                paper.Draw(Vector2.Zero);
                Draw.SpriteBatch.End();
            }

            public IEnumerator EaseIn() {
                Audio.Play("event:/game/03_resort/memo_in");
                Vector2 center = new Vector2(Engine.Width, Engine.Height) / 2f;
                Vector2 from = center + new Vector2(0f, 200f);
                Vector2 to = center;
                float rFrom = -0.1f;
                float rTo = 0.05f;
                for (float p = 0f; p < 1f; p += Engine.DeltaTime) {
                    Position = from + ((to - from) * Ease.CubeOut(p));
                    alpha = Ease.CubeOut(p);
                    rotation = rFrom + ((rTo - rFrom) * Ease.CubeOut(p));
                    yield return null;
                }
            }

            public IEnumerator EaseOut() {
                Audio.Play("event:/game/03_resort/memo_out");
                easingOut = true;
                Vector2 from = Position;
                Vector2 to = (new Vector2(Engine.Width, Engine.Height) / 2f) + new Vector2(0f, -200f);
                float rFrom = rotation;
                float rTo = rotation + 0.1f;
                for (float p = 0f; p < 1f; p += Engine.DeltaTime * 1.5f) {
                    Position = from + ((to - from) * Ease.CubeIn(p));
                    alpha = 1f - Ease.CubeIn(p);
                    rotation = rFrom + ((rTo - rFrom) * Ease.CubeIn(p));
                    yield return null;
                }

                RemoveSelf();
            }
        }
    }
}