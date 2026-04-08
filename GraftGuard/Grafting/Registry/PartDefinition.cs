using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using System.Text.Json.Serialization;

namespace GraftGuard.Grafting.Registry;

public enum PartType
{
    Limb,
    Head,
}
/// <summary>
/// Record of data for a single part
/// </summary>
public class PartDefinition
{
    //public static Texture2D TexturePlaceholderArm;
    //public static Texture2D TexturePlaceholderKnife;
    public readonly Vector2 Size = new Vector2(16, 24);
    public string Name { get; set; }
    public PartType Type { get; set; }
    public float BaseDamage { get; set; } 
    public float SpeedModifier { get; set; }
    public float ArmorModifier { get; set; }
    public float RangeModifier { get; set; }
    public float CriticalModifier { get; set; }
    public float HealthModifier { get; set; }
    public string TextureName { get; set; }
    public float PivotX { get; set; }
    public float PivotY { get; set; }
    public string[] PartBehaviorNames { get; set; }

    [JsonIgnore]
    public Texture2D Texture { get; set; }

    ///// <summary>
    ///// Loads static content for all parts
    ///// </summary>
    ///// <param name="content"></param>
    //public void LoadContent(ContentManager content)
    //{
    //    Texture = content.Load<Texture2D>($"Parts/{TextureName}");
    //}

    public PartDefinition() { }

    public PartDefinition(string name, Texture2D headTex, Vector2 headPivot)
    {
        Name = name;
        Texture = headTex;
        TextureName = "head texture";
        Vector2 pivot = headPivot;
        Type = PartType.Head;
        BaseDamage = 1;
        SpeedModifier = 1;
        ArmorModifier = 1;
        RangeModifier = 1;
        CriticalModifier = 1;
        HealthModifier = 1;
    }


    public PartDefinition(string name, Texture2D texture, string textureName, float pivotX, float pivotY, PartType type,
                              float baseDamage, float speedModifier = 1.0f, float armorModifier = 1.0f,
                              float rangeModifier = 1.0f, float criticalModifier = 1.0f, float healthModifier = 1.0f, string[]? partBehaviorNames = null)
    {
        Name = name;
        Texture = texture;
        TextureName = textureName;
        PivotX = pivotX;
        PivotY = pivotY;
        Type = type;
        BaseDamage = baseDamage;
        SpeedModifier = speedModifier;
        ArmorModifier = armorModifier;
        RangeModifier = rangeModifier;
        CriticalModifier = criticalModifier;
        HealthModifier = healthModifier;
        PartBehaviorNames = partBehaviorNames ?? [];
    }

   
}
