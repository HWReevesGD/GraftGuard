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
    public Vector2 Size { get; set; }
    public Rectangle ReceptacleBounds { get; set; }
    public Garage()
    {
        MainPatch = new NinePatch(Placeholders.TextureGaragePatch, 15, 15, 15, 15);
        Position = new Vector2(312, 960);
        Size = new Vector2(480, 320);
        ReceptacleBounds = new Rectangle((Position + new Vector2(Size.X * 0.5f - (96 * 0.5f), Size.Y - 64)).ToPoint(), new Point(96, 64));
    }
    public void Update(GameTime time, World world)
    {
        // If the player overlaps the Part Receptacle, collect the player's parts
        if (world.Player.Hitbox.Intersects(ReceptacleBounds))
        {
            foreach (PartDefinition part in world.Player.HeldParts)
            {
                world.TowerGrafter.ModifyPartCount(part, 1);
            }
            world.Player.ClearHeldParts();
        }
    }
    public void Draw(SpriteBatch batch, GameTime time)
    {
        MainPatch.Draw(batch, Position, Size);
        batch.Draw(Placeholders.TexturePartReceptacle, ReceptacleBounds, Color.White);
    }
}
