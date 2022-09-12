using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.ShroomHelper.Triggers {
    [CustomEntity("ShroomHelper/MultilayerMusicFadeTrigger")]
    public class MultilayerMusicFadeTrigger : Trigger {

        public string trackEvent;
        public bool persistent;
        public bool destroyOnLeave;

        public string P1;
        public float P1From;
        public float P1To;
        public PositionModes P1Direction;

        public string P2;
        public float P2From;
        public float P2To;
        public PositionModes P2Direction;

        public string P3;
        public float P3From;
        public float P3To;
        public PositionModes P3Direction;

        private EntityID id;

        public MultilayerMusicFadeTrigger(EntityData data, Vector2 offset) 
            : base(data, offset) {
            id = new EntityID(data.Level.Name, data.ID);

            trackEvent = data.Attr("trackEvent", "");

            P1From = data.Float("P1From");
            P1To = data.Float("P1To", 1f);
            P1 = data.Attr("P1");
            P1Direction = data.Enum("P1Direction", PositionModes.NoEffect);

            P2From = data.Float("P2From");
            P2To = data.Float("P2To", 1f);
            P2 = data.Attr("P2");
            P2Direction = data.Enum("P2Direction", PositionModes.NoEffect);

            P3From = data.Float("P3From");
            P3To = data.Float("P3To", 1f);
            P3 = data.Attr("P3");
            P3Direction = data.Enum("P3Direction", PositionModes.NoEffect);

            destroyOnLeave = data.Bool("destroyOnLeave");
            persistent = data.Bool("persistent");
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            if (!string.IsNullOrEmpty(trackEvent)) {
                Session session = SceneAs<Level>().Session;
                session.Audio.Music.Event = SFX.EventnameByHandle(trackEvent);
                session.Audio.Apply(forceSixteenthNoteHack: false);
            }
        }

        public override void OnStay(Player player) {
            float parameterValue1 = MathHelper.Lerp(P1From, P1To, GetPositionLerp(player, P1Direction));
            if (!string.IsNullOrEmpty(P1)) {
                Audio.SetMusicParam(P1, parameterValue1);
            }

            float parameterValue2 = MathHelper.Lerp(P2From, P2To, GetPositionLerp(player, P2Direction));
            if (!string.IsNullOrEmpty(P2)) {
                Audio.SetMusicParam(P2, parameterValue2);
            }

            float parameterValue3 = MathHelper.Lerp(P3From, P3To, GetPositionLerp(player, P3Direction));
            if (!string.IsNullOrEmpty(P3)) {
                Audio.SetMusicParam(P3, parameterValue3);
            }
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);

            if (destroyOnLeave) {
                Remove();
            }
        }

        public void Remove() {
            RemoveSelf();

            if (persistent) {
                Level level = SceneAs<Level>();
                level.Session.DoNotLoad.Add(id);
            }
        }
    }
}