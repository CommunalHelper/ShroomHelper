using System;
using System.Collections.Generic;
using Celeste.Mod.ShroomHelper.Entities;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.ShroomHelper.Entities
{
    [CustomEntity("ShroomHelper/ShroomBookInteraction")]
    public class ShroomBookInteraction : Entity
    {
        public const string FlagPrefix = "it_";

        public TalkComponent Talker;

        public string assetKey;

        private float timeout = 0f;

        private bool used = false;

        public ShroomBookInteraction(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {

            base.Collider = new Hitbox(data.Width, data.Height);
            assetKey = data.Attr("assetKey");

            Vector2 drawAt = new Vector2(data.Width / 2, 0f);
            if (data.Nodes.Length != 0)
            {
                drawAt = data.Nodes[0] - data.Position;
            }
            Add(Talker = new TalkComponent(new Rectangle(0, 0, data.Width, data.Height), drawAt, OnTalk));
            Talker.PlayerMustBeFacing = false;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Session session = (scene as Level).Session;
        }

        public void OnTalk(Player player)
        {
            if (used)
            {
                return;
            }

            bool flag = true;
            base.Scene.Add(new ShroomBook(player, assetKey));
            flag = false;

            if (flag)
            {
                (base.Scene as Level).Session.SetFlag("it_" + assetKey);
                used = true;
                timeout = 0.25f;
            }
        }

        public override void Update()
        {
            if (used)
            {
                timeout -= Engine.DeltaTime;
                if (timeout <= 0f)
                {
                    RemoveSelf();
                }
            }

            base.Update();
        }
    }
}
