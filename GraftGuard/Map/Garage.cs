using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard.Map;
internal class Garage
{
    public readonly NinePatch MainPatch;
    public Vector2 Position { get; set; }
    public Vector2 Center => Position + Size * 0.5f;
    public readonly Vector2 Size = new Vector2(480, 320);
    public Rectangle ReceptacleBounds { get; set; }
    public Rectangle GameOverBounds { get; set; }
    public Garage(MapDefinition map)
    {
        MainPatch = new NinePatch(Placeholders.TextureGaragePatch, 15, 15, 15, 15);
        Position = map.GaragePosition;
        ReceptacleBounds = new Rectangle((Position + new Vector2(Size.X * 0.5f - (96 * 0.5f), Size.Y - 64)).ToPoint(), new Point(96, 64));
        GameOverBounds = new Rectangle((Position + new Vector2(0, 64)).ToPoint(), (Size - new Vector2(0, 64)).ToPoint());
    }
    public void Update(GameTime time, World world)
    {
        // If the player overlaps the Part Receptacle, collect the player's parts
        if (world.Player.Hitbox.Intersects(ReceptacleBounds))
        {
            foreach (PartDefinition part in world.Player.HeldParts)
            {
                world.Inventory.ModifyPartCount(part, 1);
            }
            world.Player.ClearHeldParts();
        }
    }
    public void Draw(DrawManager drawing, GameTime time)
    {
        MainPatch.Draw(drawing, Position, Size, isUi: false, sortMode: SortMode.Top);
        drawing.Draw(Placeholders.TexturePartReceptacle, ReceptacleBounds, sortMode: SortMode.Bottom, isUi: false);
    }
}
