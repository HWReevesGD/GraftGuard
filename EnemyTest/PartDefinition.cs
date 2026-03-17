using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyTest
{
    /// <summary>
    /// Represents the immutable data for a part loaded from the Grafter tool.
    /// </summary>
    public class PartDefinition
    {
        public string Name { get; set; }
        public Texture2D Texture { get; set; }

        // Stats
        public float BaseDamage { get; set; }
        public float SpeedModifier { get; set; }
        public float ArmorModifier { get; set; }
        public float RangeModifier { get; set; }
        public float CriticalModifier { get; set; }
        public float HealthModifier { get; set; }

        // The "Hinge" point for procedural rotation (Normalized 0.0 to 1.0)
        public Vector2 Pivot { get; set; }

        public PartDefinition(string name, Texture2D texture, Vector2 pivot)
        {
            Name = name;
            Texture = texture;
            Pivot = pivot;
        }
    }
}
