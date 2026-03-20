using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grafter
{
    public class GraftLibrary
    {
        public List<PartDefinition> Parts { get; set; } = [];
        public List<BaseDefinition> Bases { get; set; } = [];
    }

    public class BaseDefinition
    {
        public string Name { get; set; } = "New Base";
        public string TextureName { get; set; } = "";
        public bool IsTorso { get; set; } // true = Torso, false = Tower Base
        public string FullImagePath { get; set; } = "";

        public List<AttachPoint> AttachPoints { get; set; } = [];

        // This makes the name show up correctly in the ListBox
        public override string ToString() => $"{Name}";
    }

    public enum PartType
    {
        Limb,
        Head,
    }

    public struct AttachPoint {
        public string Name;
        public float PivotX { get; set; }
        public float PivotY { get; set; }
    }

    public class PartDefinition
    {
        //public static Texture2D TexturePlaceholderArm;
        //public static Texture2D TexturePlaceholderKnife;

        public string Name { get; set; }
        public PartType Type { get; set; }
        public float BaseDamage { get; set; }
        public float SpeedModifier { get; set; }
        public float ArmorModifier { get; set; }
        public float RangeModifier { get; set; }
        public float CriticalModifier { get; set; }
        public float HealthModifier { get; set; }
        public string TextureName { get; set; }
        public string FullImagePath { get; set; }
        public float PivotX { get; set; }
        public float PivotY { get; set; }


        public override string ToString() => Name;
    }


}