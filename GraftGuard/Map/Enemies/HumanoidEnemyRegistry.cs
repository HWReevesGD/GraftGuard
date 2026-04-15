using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework;
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
        public static Dictionary<int, List<PartDefinition>> Parts = new();
        private static Random random = new Random();

        public static void Load(ContentManager content)
        {
            for (int i = 0; i <= 4; i++)
            {

                var enemySockets = new List<AttachPoint>
                {
                    new AttachPoint { Name = "Arm_R", PivotX = 0.6859326f, PivotY = 0.28445747f },
                    new AttachPoint { Name = "Arm_L", PivotX = 0.31524926f, PivotY = 0.28445747f },
                    new AttachPoint { Name = "Leg_L", PivotX = 0.29765397f, PivotY = 0.7683284f },
                    new AttachPoint { Name = "Leg_R", PivotX = 0.66129035f, PivotY = 0.7859238f },
                    new AttachPoint { Name = "Head",  PivotX = 0.47947213f, PivotY = 0.199648f },
                };

                Texture2D torsoTex = content.Load<Texture2D>($"Entities/Enemies/Torsos/enemy_torso_{i}");
                Torsos[i] = new BaseDefinition($"EnemyTorso_{i}", $"enemy_torso_{i}", torsoTex, true, enemySockets);

                List<PartDefinition> typeParts = new List<PartDefinition>();


                Texture2D armTex = content.Load<Texture2D>($"Entities/Enemies/Arms/enemy_arm_{i}");
                Texture2D headTex = content.Load<Texture2D>($"Entities/Enemies/Heads/enemy_faces_{i}");
                Texture2D legTex = content.Load<Texture2D>($"Entities/Enemies/Legs/enemy_leg_{i}");

                // Head
                typeParts.Add(new PartDefinition($"Head", headTex, $"enemy_faces_{i}", 0.53f, 0.77f, PartType.Head, new Damage(1, 0, 0, 0, 0), flipHorizonal: false));

                // Arms
                typeParts.Add(new PartDefinition($"Arm_R", armTex, $"enemy_arm_{i}", 0.48f, 0.24f, PartType.Limb, new Damage(1, 0, 0, 0, 0), flipHorizonal: false));
                typeParts.Add(new PartDefinition($"Arm_L", armTex, $"enemy_arm_{i}", 0.48f, 0.24f, PartType.Limb, new Damage(1, 0, 0, 0, 0), flipHorizonal: true));

                // Legs
                typeParts.Add(new PartDefinition($"Leg_R", legTex, $"enemy_leg_{i}", 0.48f, 0.26f, PartType.Limb, new Damage(1, 0, 0, 0, 0), flipHorizonal: false));
                typeParts.Add(new PartDefinition($"Leg_L", legTex, $"enemy_leg_{i}", 0.48f, 0.26f, PartType.Limb, new Damage(1, 0, 0, 0, 0), flipHorizonal: true));

                Parts[i] = typeParts;

            }
        }

        /// <summary>
        /// Returns all parts associated with a specific enemy set index.
        /// </summary>
        public static List<PartDefinition> GetPartsForType(int type)
        {
            if (Parts.ContainsKey(type)) return Parts[type];
            return new List<PartDefinition>();
        }

        internal static (BaseDefinition Torso, int Type) GetRandomSet()
        {
            int type = random.Next(Torsos.Count);
            return (Torsos[type], type);
        }
    }
}
