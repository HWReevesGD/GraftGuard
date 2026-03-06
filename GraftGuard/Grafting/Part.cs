using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting
{
    internal class Part : GameObject
    {
        public float BaseDamge;
        public float SpeedModifier;
        public float ArmorModifier;
        public float RangeModifier;
        public float CriticalModifier;
        public float HealthModifier;

        public Part(Vector2 position, Vector2 size, Texture2D texture) : base(position, size, texture)
        {
        }
    }
}
