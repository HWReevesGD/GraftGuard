using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GraftGuard.UI;

internal class ScrollingGrid<T> where T : class, IPositional, ISizeable
{
    private readonly Vector2 _arrowHorizontalSize = new Vector2(64, 32);
    private readonly Vector2 _arrowVerticalSize = new Vector2(32, 64);
    public const float ScrollSpeed = 256.0f;
    public Vector2 Position { get; init; }
    public Vector2 Size { get; init; }
    public Rectangle GridRectangle => new Rectangle(Position.ToPoint(), Size.ToPoint());
    public Vector2 ElementSize { get; init; }
    public List<T> Elements { get; set; }
    public Orientation Orientation { get; init; }
    public Corner ArrowSide { get; init; }
    public float ArrowOffset { get; init; }
    public Corner SubgridSide { get; init; }
    public float ScrollAmount { get; private set; }
    public bool IsScrollActive { get; private set; }
    public bool IsMouseOver => GridRectangle.Contains(_inputManager?.CurrentMouse.Position ?? Mouse.GetState().Position);
    private PatchButton _negativeArrow;
    private PatchButton _positiveArrow;
    private InputManager _inputManager;
    public ScrollingGrid(Orientation orientation, Vector2 gridPosition, Vector2 gridSize, Vector2 elementSize, Corner arrowSide, float arrowOffset, Corner subgridSide)
    {
        Elements = [];
        Orientation = orientation;
        Position = gridPosition;
        Size = gridSize;
        ElementSize = elementSize;
        ArrowSide = arrowSide;
        ArrowOffset = arrowOffset;
        ScrollAmount = 0.0f;
        IsScrollActive = false;
        SubgridSide = subgridSide;

        MakeArrows(orientation, arrowSide, arrowOffset);
    }
    /// <summary>
    /// Updates the <see cref="ScrollingGrid{T}"/>. <paramref name="updateElement"/> can be used to run an <see cref="Action"/> on each element for updating
    /// </summary>
    /// <param name="updateElement"><see cref="Action"/> to run on each element, with the element and the index provided</param>
    public void Update(GameTime time, InputManager input, Action<T, int> updateElement = null)
    {
        _inputManager = input;
        // Get values for the current state
        float orientSize = GetAlong(Size, Orientation);
        float elementOrientSize = GetAlong(ElementSize, Orientation);
        int subAmount = GetElementsPerSubGrid();
        int superAmount = GetSuperGridCount();
        float superSize = elementOrientSize * superAmount;

        // Calculate how much "beyond" the grid the elements go
        float elementsBeyondAmount = MathF.Max(0.0f, superSize - orientSize);
        IsScrollActive = elementsBeyondAmount > 0.0f;

        if (IsScrollActive)
        {
            // Update Arrows
            _negativeArrow.Update(input);
            _positiveArrow.Update(input);

            // Get Frame Delta
            float delta = time.Delta();

            // Scroll
            if (_negativeArrow.IsPressed)
            {
                ScrollAmount -= delta * ScrollSpeed;
            }
            if (_positiveArrow.IsPressed)
            {
                ScrollAmount += delta * ScrollSpeed;
            }
        }

        // Clamp Scroll Amount
        ScrollAmount = MathHelper.Clamp(ScrollAmount, 0.0f, elementsBeyondAmount);

        // Update Elements
        for (int index = 0; index < Elements.Count; index++)
        {
            T element = Elements[index];

            PositionElement(element, index);

            // Update Element Action
            updateElement?.Invoke(element, index);
        }
    }

    /// <summary>
    /// Draws the <see cref="ScrollingGrid{T}"/>. <paramref name="drawElement"/> can be used to run an <see cref="Action"/> on each element for drawing
    /// </summary>
    /// <param name="drawing"><see cref="SpriteBatch"/> to use</param>
    /// <param name="drawElement"><see cref="Action"/> to run on each element, with the element and index provided</param>
    public void Draw(DrawManager drawing, Action<DrawManager, T, int> drawElement = null, bool skipBackground = false)
    {
        // Draw Back
        if (!skipBackground)
        {
            drawing.Draw(Placeholders.TexturePixel, destination: new Rectangle(Position.ToPoint(), Size.ToPoint()), color: new Color(Color.Black, 0.3f));
        }

        // Draw Arrows
        if (IsScrollActive)
        {
            _negativeArrow.Draw(drawing);
            _positiveArrow.Draw(drawing);
        }

        if (drawElement is null)
        {
            return;
        }

        // Draw Elements
        for (int index = 0; index < Elements.Count; index++)
        {
            T element = Elements[index];
            drawElement?.Invoke(drawing, element, index);
        }
    }

    /// <summary>
    /// Positions the given <paramref name="element"/> in it's place based on the given <paramref name="index"/> and the state of the <see cref="ScrollingGrid{T}"/>
    /// </summary>
    /// <param name="element">Element to position</param>
    /// <param name="index">Index of the element</param>
    private void PositionElement(T element, int index)
    {
        int subAmount = GetElementsPerSubGrid();
        element.Size = ElementSize;

        // Arrange Element
        element.Position = ElementSize * (index / subAmount) - Vector2.One * ScrollAmount;
        float perpendicularElementSize = GetPerpendicular(ElementSize, Orientation);
        float subPosition = (index % subAmount) * perpendicularElementSize;

        // Set element's position X or Y to it's sub position, depending on orientation and subgrid side
        switch (Orientation)
        {
            case Orientation.Horizontal:
                switch (SubgridSide)
                {
                    case Corner.TopOrRight:
                        element.Position = element.Position with { Y = subPosition };
                        break;
                    case Corner.BottomOrLeft:
                        element.Position = element.Position with { Y = GetPerpendicular(Size, Orientation) - subPosition - perpendicularElementSize};
                        break;
                }
                break;
            case Orientation.Vertical:
                switch (SubgridSide)
                {
                    case Corner.TopOrRight:
                        element.Position = element.Position with { X = subPosition };
                        break;
                    case Corner.BottomOrLeft:
                        element.Position = element.Position with { X = GetPerpendicular(Size, Orientation) - subPosition - perpendicularElementSize};
                        break;
                }
                break;
        }

        // Move the element to the grid's position
        element.Position += Position;
    }

    /// <summary>
    /// Calculates how many elements will fit next to each other, before descending down this scroll grid
    /// </summary>
    /// <returns>Amount of elements per sub-grid calculated</returns>
    public int GetElementsPerSubGrid()
    {
        return (int)(GetPerpendicular(Size, Orientation) / GetPerpendicular(ElementSize, Orientation));
    }

    /// <summary>
    /// Calculates how many sub-grids are currently in the scrolling grid
    /// </summary>
    /// <returns>Amount of sub-grids calculated</returns>
    public int GetSuperGridCount()
    {
        return (int)MathF.Ceiling(Elements.Count / (float)GetElementsPerSubGrid());
    }

    /// <summary>
    /// Adds a <see cref="T"/> element to <see cref="Elements"/>
    /// </summary>
    /// <param name="element">Element to add</param>
    public void Add(T element)
    {
        Elements.Add(element);
    }

    /// <summary>
    /// Removes the given <see cref="T"/> element
    /// </summary>
    /// <param name="element">Element to remove</param>
    /// <returns>Returns true if the element was removed successfully</returns>
    public bool Remove(T element)
    {
        return Elements.Remove(element);
    }

    /// <summary>
    /// Removes the <see cref="T"/> element at the given <paramref name="index"/>
    /// </summary>
    /// <param name="index">Index to remove at</param>
    public void RemoveAt(int index)
    {
        Elements.RemoveAt(index);
    }

    /// <summary>
    /// Returns the component of the given <paramref name="vector"/> that is along the given <see cref="Orientation"/>
    /// </summary>
    /// <param name="vector"><see cref="Vector2"/> to use</param>
    /// <param name="orientation"><see cref="Orientation"/> to use</param>
    /// <returns>Vector component along the orientation</returns>
    public float GetAlong(Vector2 vector, Orientation orientation)
    {
        return orientation switch
        {
            Orientation.Horizontal => vector.X,
            Orientation.Vertical => vector.Y,
            _ => throw new UnreachableException(
                "If this is thrown, it means that Orientation has been updated, and a new case was not added to this statement!"),
        };
    }

    /// <summary>
    /// Returns the component of the given <paramref name="vector"/> that is perpendicular to the given <see cref="Orientation"/>
    /// </summary>
    /// <param name="vector"><see cref="Vector2"/> to use</param>
    /// <param name="orientation"><see cref="Orientation"/> to use</param>
    /// <returns>Vector component perpendicular to the orientation</returns>
    public float GetPerpendicular(Vector2 vector, Orientation orientation)
    {
        return orientation switch
        {
            Orientation.Horizontal => vector.Y,
            Orientation.Vertical => vector.X,
            _ => throw new UnreachableException(
                "If this is thrown, it means that Orientation has been updated, and a new case was not added to this statement!"),
        };
    }

    /// <summary>
    /// Constructs the arrows for the <see cref="ScrollingGrid{T}"/>
    /// </summary>
    /// <param name="orientation">Grid <see cref="Orientation"/></param>
    /// <param name="side">Arrow side, as a <see cref="Corner"/></param>
    /// <param name="offset">Arrow offset from their original position</param>
    private void MakeArrows(Orientation orientation, Corner side, float offset)
    {
        switch (orientation)
        {
            case Orientation.Horizontal:
                switch (side)
                {
                    case Corner.TopOrRight:
                        _negativeArrow = PatchButton.MakeBase(
                            Position + new Vector2(Size.X * 0.5f, 0) - _arrowHorizontalSize + new Vector2(offset, 0),
                            _arrowHorizontalSize,
                            "<");
                        _positiveArrow = PatchButton.MakeBase(
                            Position + new Vector2(Size.X * 0.5f, 0) - Vector2.UnitY * _arrowHorizontalSize + new Vector2(offset, 0),
                            _arrowHorizontalSize,
                            ">");
                        break;
                    case Corner.BottomOrLeft:
                        _negativeArrow = PatchButton.MakeBase(
                            Position + new Vector2(Size.X * 0.5f, Size.Y) + _arrowHorizontalSize * new Vector2(-1, 0) + new Vector2(offset, 0),
                            _arrowHorizontalSize,
                            "<");
                        _positiveArrow = PatchButton.MakeBase(
                            Position + new Vector2(Size.X * 0.5f, Size.Y) + new Vector2(offset, 0),
                            _arrowHorizontalSize,
                            ">");
                        break;
                }
                break;
            case Orientation.Vertical:
                switch (side)
                {
                    case Corner.TopOrRight:
                        _negativeArrow = PatchButton.MakeBase(
                            Position + new Vector2(0, Size.Y * 0.5f) - _arrowVerticalSize + new Vector2(0, offset),
                            _arrowVerticalSize,
                            @"/\");
                        _positiveArrow = PatchButton.MakeBase(
                            Position + new Vector2(0, Size.Y * 0.5f) - _arrowVerticalSize * Vector2.UnitX + new Vector2(0, offset),
                            _arrowVerticalSize,
                            @"\/");
                        break;
                    case Corner.BottomOrLeft:
                        _negativeArrow = PatchButton.MakeBase(
                            Position + new Vector2(Size.X, Size.Y * 0.5f) - _arrowVerticalSize * Vector2.UnitY + new Vector2(0, offset),
                            _arrowVerticalSize,
                            @"/\");
                        _positiveArrow = PatchButton.MakeBase(
                            Position + new Vector2(Size.X, Size.Y * 0.5f) + new Vector2(0, offset),
                            _arrowVerticalSize,
                            @"\/");
                        break;
                }
                break;
        }
    }
}
