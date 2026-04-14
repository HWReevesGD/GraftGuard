using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GraftGuard.Grafting.Towers;

internal delegate Tower CreateTower(Vector2 position);
internal delegate void DrawPreview(DrawManager drawing, GameTime time, Vector2 position);
/// <summary>
/// Base Class for all Towers
/// </summary>
internal abstract class Tower : GameObject, IMouseDetectable
{
    public static Texture2D TexturePlaceholderTower { get; private set; }
    public static Texture2D TexturePlaceholderGround { get; private set; }
    public static Texture2D TTurret { get; private set; }
    public static Texture2D TNest { get; private set; }
    public const int MaxParts = 4;

    public static void LoadContent(ContentManager content)
    {
        TexturePlaceholderTower = content.Load<Texture2D>("Placeholder/tower_placeholder");
        TexturePlaceholderGround = content.Load<Texture2D>("Placeholder/tower_placeholder_2");
        TTurret = content.Load<Texture2D>("Tower/turret");
        TNest = content.Load<Texture2D>("Tower/nest");
    }

    protected AttachedPart[] _attachedParts;
    public PartDefinition[] Parts => _attachedParts.Select((part) => part?.Definition).ToArray();

    /// <summary>
    /// True if there is at least one non-null part attached
    /// </summary>
    public bool HasParts => _attachedParts.Any((part) => part is not null);
    public int TotalAttachedParts => _attachedParts.Count((part) => part is not null);
    /// <summary>
    /// Rectangle which determines where the Mouse will be considered to be "hovering" over this tower
    /// </summary>
    public Rectangle MouseBox { get; private set; }
    /// <summary>
    /// Random used for general <see cref="Tower"/> randomness
    /// </summary>
    protected static Random random;

    private float _basePathCost;
    public float PathCost => HasParts ? _basePathCost : 0;
    public Rectangle[] PathAreas { get; init; }
    public PartSettings Settings { get; init; }
    public int NightsUsed { get; set; }

    /// <summary>
    /// Constructs a Tower with Empty Parts
    /// </summary>
    /// <param name="position">Tower's Initial Position</param>
    /// <param name="size">Tower's Drawing Size</param>
    /// <param name="texture">Tower's Texture</param>
    public Tower(Vector2 position, Vector2 size, Texture2D texture, Rectangle mouseBox, float pathCost, Rectangle[] pathAreas = null, PartSettings? settings = null) : base(position, size, texture)
    {
        NightsUsed = 0;
        _attachedParts = new AttachedPart[MaxParts];

        random = new Random();
        MouseBox = mouseBox;
        _basePathCost = pathCost;
        
        for (int i = 0; i < pathAreas.Length; i++)
        {
            pathAreas[i] = pathAreas[i].Translated(position.ToPoint());
        }

        PathAreas = pathAreas ?? [];
        Settings = settings ?? PartSettings.DefaultTower;
    }

    /// <summary>
    /// Empty Update for use in Child Classes
    /// </summary>
    public virtual void Update(GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileDiversion = null)
    {

    }

    public virtual void Draw(GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state, bool isUi = false, SortMode defaultSortMode = SortMode.Sorted, int drawLayerOffset = 0)
    {
        drawing.Draw(Texture, Position, isUi: isUi, sortMode: defaultSortMode, drawLayer: 1 + drawLayerOffset);
    }

    /// <summary>
    /// Gets a part from an <paramref name="index"/>. If <paramref name="shiftIfNull"/> is <see cref="true"/>, then the method will attempt to choose another attached part
    /// </summary>
    /// <param name="index">Index of the part</param>
    /// <param name="shiftIfNull">Attempts to shift to another part if the part at the index is null</param>
    /// <returns></returns>
    public AttachedPart GetPart(int index, bool shiftIfNull)
    {
        AttachedPart part = _attachedParts[index];
        if (shiftIfNull && part is null)
        {
            foreach (AttachedPart otherPart in _attachedParts)
            {
                if (otherPart is not null)
                {
                    part = otherPart;
                    break;
                }
            }
        }
        return part;
    }

    /// <summary>
    /// Attaches a <see cref="PartDefinition"/> to the first empty <see cref="Slot"/>. If the <see cref="Tower"/> is full, this does nothing
    /// </summary>
    /// <param name="part"><see cref="PartDefinition"/> to attach</param>
    public void AttachPart(PartDefinition part)
    {
        for (int index = 0; index < _attachedParts.Length; index++)
        {
            if (_attachedParts[index] is null)
            {
                _attachedParts[index] = new AttachedPart(part);
                return;
            }
        }
    }

    /// <summary>
    /// Removes and returns the last attached part of the <see cref="Tower"/>
    /// </summary>
    /// <returns><see cref="PartDefinition"/> of the removed part</returns>
    public PartDefinition RemovePart()
    {

        for (int index = MaxParts - 1; index >= 0; index--)
        {
            if (_attachedParts[index] is AttachedPart part)
            {
                _attachedParts[index] = null;
                return part.Definition;
            }
        }

        return null;
    }

    /// <summary>
    /// Checks if the Mouse's Position is over this <see cref="Tower"/>
    /// </summary>
    /// <param name="inputManager"><see cref="InputManager"/> to use for the Mouse</param>
    /// <returns><see cref="true"/> if the Mouse overlaps the <see cref="Tower"/>, <see cref="false"/> otherwise</returns>
    public bool IsMouseOver(InputManager inputManager)
    {
        return (MouseBox with { X = (int)Position.X + MouseBox.X, Y = (int)Position.Y + MouseBox.Y }).Contains(inputManager.MouseWorldPosition);
    }

    /// <summary>
    /// Checks if the given Position is over this <see cref="Tower"/>. This should only be used for Mouse Detection, not collision
    /// </summary>
    /// <param name="inputManager"><see cref="InputManager"/> to use for the Mouse</param>
    /// <returns><see cref="true"/> if the Mouse overlaps the <see cref="Tower"/>, <see cref="false"/> otherwise</returns>
    public bool IsOver(Vector2 position)
    {
        return (MouseBox with { X = (int)Position.X + MouseBox.X, Y = (int)Position.Y + MouseBox.Y }).Contains(position);
    }
}
