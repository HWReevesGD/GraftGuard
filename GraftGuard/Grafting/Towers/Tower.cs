using GraftGuard.Grafting.Registry;
using GraftGuard.Map;
using GraftGuard.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace GraftGuard.Grafting.Towers;

internal delegate Tower CreateTower(Vector2 position);
internal delegate void DrawPreview(SpriteBatch batch, GameTime time, Vector2 position);
/// <summary>
/// Base Class for all Towers
/// </summary>
internal abstract class Tower : GameObject, IMouseDetectable
{
    public static Texture2D TexturePlaceholderTower { get; private set; }
    public static Texture2D TexturePlaceholderGround { get; private set; }

    public static void LoadContent(ContentManager content)
    {
        TexturePlaceholderTower = content.Load<Texture2D>("Placeholder/tower_placeholder");
        TexturePlaceholderGround = content.Load<Texture2D>("Placeholder/tower_placeholder_2");
    }

    public enum Slot
    {
        First,
        Second,
        Third,
        Fourth,
    }

    protected PartDefinition[] _attachedParts;

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

    /// <summary>
    /// Constructs a Tower with Empty Parts
    /// </summary>
    /// <param name="position">Tower's Initial Position</param>
    /// <param name="size">Tower's Drawing Size</param>
    /// <param name="texture">Tower's Texture</param>
    public Tower(Vector2 position, Vector2 size, Texture2D texture, Rectangle mouseBox) : base(position, size, texture)
    {
        _attachedParts = new PartDefinition[4];
        random = new Random();
        MouseBox = mouseBox;
    }

    /// <summary>
    /// Empty Update for use in Child Classes
    /// </summary>
    public virtual void Update(GameTime time, World world, InputManager inputManager, TimeState state)
    {

    }

    /// <summary>
    /// Gets a part from an <paramref name="index"/>. If <paramref name="shiftIfNull"/> is <see cref="true"/>, then the method will attempt to choose another attached part
    /// </summary>
    /// <param name="index">Index of the part</param>
    /// <param name="shiftIfNull">Attempts to shift to another part if the part at the index is null</param>
    /// <returns></returns>
    public PartDefinition GetPartFromIndex(int index, bool shiftIfNull)
    {
        PartDefinition part = _attachedParts[index];
        if (shiftIfNull && part is null)
        {
            foreach (PartDefinition otherPart in _attachedParts)
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
    /// Sets the part at the given <see cref="Slot"/> to the given <paramref name="part"/>
    /// </summary>
    /// <param name="part">Part to set</param>
    /// <param name="slot"><see cref="Slot"/> to use</param>
    public virtual void SetPart(PartDefinition part, Slot slot)
    {
        switch (slot)
        {
            case Slot.First:
                _attachedParts[0] = part;
                break;
            case Slot.Second:
                _attachedParts[1] = part;
                break;
            case Slot.Third:
                _attachedParts[2] = part;
                break;
            case Slot.Fourth:
                _attachedParts[3] = part;
                break;
        }
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
                _attachedParts[index] = part;
                return;
            }
        }
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
}
