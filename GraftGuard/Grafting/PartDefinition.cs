using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting;

enum PartType
{
    Limb,
    Head,
}
internal class PartDefinition
{
    public static Texture2D TexturePlaceholderArm;
    public static Texture2D TexturePlaceholderKnife;

    public readonly string Name;
    public readonly PartType Type;
    public readonly float BaseDamge;
    public readonly float SpeedModifier;
    public readonly float ArmorModifier;
    public readonly float RangeModifier;
    public readonly float CriticalModifier;
    public readonly float HealthModifier;
    public readonly Texture2D Texture;

    public static void LoadContent(ContentManager content)
    {
        TexturePlaceholderArm = content.Load<Texture2D>("Placeholder/placeholder_part_1");
        TexturePlaceholderKnife = content.Load<Texture2D>("Placeholder/placeholder_part_2");
    }

    public PartDefinition(string name, Texture2D texture, PartType type, float baseDamage, float speedModifier = 1.0f, float armorModifier = 1.0f, float rangeModifier = 1.0f, float criticalModifier = 1.0f, float healthModifier = 1.0f)
    {
        Name = name;
        Type = type;
        Texture = texture;
        BaseDamge = baseDamage;
        SpeedModifier = speedModifier;
        ArmorModifier = armorModifier;
        RangeModifier = rangeModifier;
        CriticalModifier = criticalModifier;
        HealthModifier = healthModifier;
    }
}
