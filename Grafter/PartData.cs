using Grafter.Tools;
using System.ComponentModel;

namespace Grafter
{
    public interface IGameDefinition : ITextureEditor
    {
        string Name { get; set; }

        void InitializeDefault();
    }
    public class GraftLibrary
    {
        public List<PartDefinition> Parts { get; set; } = [];
        public List<BaseDefinition> Bases { get; set; } = [];
    }

    public class BaseDefinition : IGameDefinition
    {
        public string Name { get; set; }
        public string TextureName { get; set; }
        public bool IsTorso { get; set; } // true = Torso, false = Tower Base
        public string FullImagePath { get; set; }

        public BindingList<AttachPoint> AttachPoints { get; set; } = [];

        // This makes the name show up correctly in the ListBox
        public override string ToString() => $"{Name}";

        public void InitializeDefault() => Name = "New Base";
    }

    public enum PartType
    {
        Limb,
        Head,
    }

    public class AttachPoint {
        public string Name { get; set; } = "New Socket";
        public float PivotX { get; set; }
        public float PivotY { get; set; }

        public int ColorArgb { get; set; } = Color.Red.ToArgb();
    }

    public class PartDefinition : IGameDefinition
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
        public float Rarity { get; set; } = 1.0f;
        public string TextureName { get; set; }
        public string FullImagePath { get; set; }
        public float PivotX { get; set; }
        public float PivotY { get; set; }
        public string[] PartBehaviorNames { get; set; } = [];


        public override string ToString() => Name;

        public void InitializeDefault() => Name = "New Base";
    }


}