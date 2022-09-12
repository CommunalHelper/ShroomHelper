using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.ShroomHelper.Triggers
{
    [CustomEntity("ShroomHelper/GradualChangeColorGradeTrigger")]
    class GradualChangeColorGradeTrigger : Trigger
    {
        private string colorGrade;
        public float speed;

        public GradualChangeColorGradeTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            colorGrade = data.Attr("colorGrade", "none");
            speed = data.Float("speed", 1);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (colorGrade == "none")
            {
                colorGrade = null;
            }
            (base.Scene as Level)?.NextColorGrade(colorGrade, speed);
        }
    }
}
