using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting
{
    internal class PartDefinition
    {
        public static Texture2D TexturePlaceholder1;

        public float BaseDamge;
        public float SpeedModifier;
        public float ArmorModifier;
        public float RangeModifier;
        public float CriticalModifier;
        public float HealthModifier;

        public Texture2D Texture { get; set; }

        public static void LoadContent(ContentManager content)
        {
            TexturePlaceholder1 = content.Load<Texture2D>("Placeholder/placeholder_part_1");
        }

        public PartDefinition(Texture2D texture, float baseDamage, float speedModifier = 1.0f, float armorModifier = 1.0f, float rangeModifier = 1.0f, float criticalModifier = 1.0f, float healthModifier = 1.0f)
        {
            Texture = texture;
        }
    }
}
