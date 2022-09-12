using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.ShroomHelper.Entities {
    [CustomEntity("ShroomHelper/ShroomBookInteraction")]
    public class ShroomBookInteraction : Entity {
        public const string FlagPrefix = "it_";
        public TalkComponent Talker;
        public string assetKey;

        public ShroomBookInteraction(EntityData data, Vector2 offset)
            : base(data.Position + offset) {

            Collider = new Hitbox(data.Width, data.Height);
            assetKey = data.Attr("assetKey", "shroompage");

            Vector2 drawAt = new(data.Width / 2, 0f);
            if (data.Nodes.Length != 0) {
                drawAt = data.Nodes[0] - data.Position;
            }

            Add(Talker = new TalkComponent(new Rectangle(0, 0, data.Width, data.Height), drawAt, OnTalk));
            Talker.PlayerMustBeFacing = false;
        }

        public void OnTalk(Player player) {
            Scene.Add(new ShroomBook(player, assetKey));
        }
    }
}
