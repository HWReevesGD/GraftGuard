using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Enemies
{
    public static class HumanoidEnemyRegistry
    {
        public static Dictionary<int, BaseDefinition> Torsos = new();
        public static Dictionary<int, List<Texture2D>> Parts = new(); 

        public static void Load(ContentManager content)
        {
            for (int i = 0; i <= 4; i++)
            {

                var playerSockets = new List<AttachPoint>
                {
                    new AttachPoint { Name = "Arm_R", PivotX = 0.6859326f, PivotY = 0.28445747f },
                    new AttachPoint { Name = "Arm_L", PivotX = 0.31524926f, PivotY = 0.28445747f },
                    new AttachPoint { Name = "Leg_L", PivotX = 0.29765397f, PivotY = 0.7683284f },
                    new AttachPoint { Name = "Leg_R", PivotX = 0.66129035f, PivotY = 0.7859238f },
                    new AttachPoint { Name = "Head",  PivotX = 0.47947213f, PivotY = 0.199648f },
                    new AttachPoint { Name = "Ponytail", PivotX = 0.15f, PivotY = -0.15f },
                };

                Texture2D torsoTex = content.Load<Texture2D>($"Entities/Enemies/Torsos/enemy_torso_{i}");
                //Torsos[i] = new BaseDefinition(torsoTex);

                Texture2D armTex = content.Load<Texture2D>($"Entities/Enemies/Torsos/enemy_arm_{i}");
                Texture2D headTex = content.Load<Texture2D>($"Entities/Enemies/Torsos/enemy_face_{i}");
                Texture2D legTex = content.Load<Texture2D>($"Entities/Enemies/Torsos/enemy_leg_{i}");
            }
        }
    }
}
