using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafter
{
    public class PartData
    {
        public string Name { get; set; } = "New Part";
        public float BaseDamage { get; set; }
        public float SpeedModifier { get; set; }
        public float ArmorModifier { get; set; }
        public float RangeModifier { get; set; }
        public float CriticalModifier { get; set; }
        public float HealthModifier { get; set; }

        // New fields for the game engine
        public string TextureName { get; set; } = "";

        // Default to center
        public float PivotX { get; set; } = 0.5f; 
        public float PivotY { get; set; } = 0.5f;

        public string FullImagePath { get; set; } = ""; 

        public string ToCsv() => $"{Name},{BaseDamage},{SpeedModifier},{ArmorModifier},{RangeModifier},{CriticalModifier},{HealthModifier},{TextureName},{PivotX},{PivotY}";


        public static PartData FromCsv(string csvLine)
        {
            var parts = csvLine.Split(',');
            return new PartData
            {
                Name = parts[0],
                BaseDamage = float.Parse(parts[1]),
                SpeedModifier = float.Parse(parts[2]),
                ArmorModifier = float.Parse(parts[3]),
                RangeModifier = float.Parse(parts[4]),
                CriticalModifier = float.Parse(parts[5]),
                HealthModifier = float.Parse(parts[6]),
                TextureName = parts.Length > 7 ? parts[7] : "",
                PivotX = parts.Length > 8 ? float.Parse(parts[8]) : 0.5f,
                PivotY = parts.Length > 9 ? float.Parse(parts[9]) : 0.5f
            };
        }

        public override string ToString() => Name;
    }
}