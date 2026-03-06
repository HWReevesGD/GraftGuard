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

        public string TextureName { get; set; } = ""; 

        public string FullImagePath { get; set; } = ""; 

        // Converts the object to a CSV line
        public string ToCsv() => $"{Name},{BaseDamage},{SpeedModifier},{ArmorModifier},{RangeModifier},{CriticalModifier},{HealthModifier}";

        // Creates an object from a CSV line
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
                HealthModifier = float.Parse(parts[6])
            };
        }

        public override string ToString() => Name;
    }
}
