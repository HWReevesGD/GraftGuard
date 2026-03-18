using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry;

enum PartType
{
    Limb,
    Head,
}
/// <summary>
/// Record of data for a single part
/// </summary>
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

    /// <summary>
    /// Loads static content for all parts
    /// </summary>
    /// <param name="content"></param>
    public static void LoadContent(ContentManager content)
    {
        TexturePlaceholderArm = content.Load<Texture2D>("Placeholder/placeholder_part_1");
        TexturePlaceholderKnife = content.Load<Texture2D>("Placeholder/placeholder_part_2");
    }

    /// <summary>
    /// Creates a new <see cref="PartDefinition"/> with the given data
    /// </summary>
    /// <param name="name">Part name</param>
    /// <param name="texture">Part texture</param>
    /// <param name="type">Part Type, either Limb or Head</param>
    /// <param name="baseDamage">Base damage of the part</param>
    /// <param name="speedModifier">Part speed modifier</param>
    /// <param name="armorModifier">Part armor modifier</param>
    /// <param name="rangeModifier">Part range modifier</param>
    /// <param name="criticalModifier">Part critical hit chance multiplier</param>
    /// <param name="healthModifier">Part health modifier</param>
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
