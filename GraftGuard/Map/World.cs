using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.Map.Enemies;
using GraftGuard.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GraftGuard.Map;
internal class World
{
    // Fields
    private List<PathNode> pathNodes;
    private List<Enemy> enemies;

    // Properties
    public Player Player { get; set; }
    public TowerManager TowerManager { get; set; }
    public TowerGrafter TowerGrafter { get; set; }
    public Terrain Terrain { get; set; }
    public Camera Camera { get; set; }
    public Garage Garage { get; set; }
    public List<ScatteredPart> ScatteredParts { get; set; }

    // Constructor
    public World()
    {
        pathNodes = new List<PathNode>();
        enemies = new List<Enemy>();

        // These parts are just for testing, this will normally start empty
        ScatteredParts = [
            new ScatteredPart(new Vector2(420, 220), PartRegistry.GetByName("arm")),
            new ScatteredPart(new Vector2(420, 320), PartRegistry.GetByName("knife")),
            new ScatteredPart(new Vector2(520, 320), PartRegistry.GetByName("arm")),
            new ScatteredPart(new Vector2(620, 320), PartRegistry.GetByName("knife")),
            new ScatteredPart(new Vector2(720, 320), PartRegistry.GetByName("knife")),
            new ScatteredPart(new Vector2(420, 220), PartRegistry.GetByName("arm")),
            new ScatteredPart(new Vector2(420, 220), PartRegistry.GetByName("knife")),
            new ScatteredPart(new Vector2(520, 220), PartRegistry.GetByName("arm")),
            new ScatteredPart(new Vector2(620, 220), PartRegistry.GetByName("knife")),
            new ScatteredPart(new Vector2(720, 220), PartRegistry.GetByName("knife")),
            ];

        TowerManager = new TowerManager();
        TowerGrafter = new TowerGrafter(TowerManager);
        Terrain = new Terrain();
        Garage = new Garage();

        Player = new Player(Vector2.Zero);
        Camera = new Camera();
    }

    // Methods
    public void Update(GameTime gameTime, InputManager inputManager, TimeState state)
    {
        switch (state)
        {
            case TimeState.Night:
                Player.Update(gameTime, inputManager, this);
                break;
            case TimeState.Dawn:
                Player.Update(gameTime, inputManager, this);
                break;
            case TimeState.Day:
                Player.Position = Garage.Center;
                Camera.UpdateFreeMovement(gameTime, inputManager);
                break;
        }

        inputManager.Update(Camera);
        TowerManager.Update(gameTime);
        Terrain.Update(gameTime);
        Garage.Update(gameTime, this);

        if (state == TimeState.Day)
        {
            TowerGrafter.Update(gameTime, inputManager, this);
        }
    }

    public void DrawStatic(SpriteBatch batch, GameTime gameTime, TimeState state)
    {
        batch.DrawString(Fonts.Arial, $"Mouse Screen: {Mouse.GetState().Position.ToVector2()}\nMouse World: {Vector2.Transform(Mouse.GetState().Position.ToVector2(), Camera.ScreenToWorld)}", new Vector2(64, 128), Color.White);
        if (state == TimeState.Day)
        {
            TowerGrafter.Draw(batch, gameTime);
        }
    }

    public void DrawCamera(SpriteBatch batch, GameTime gameTime, TimeState state)
    {
        Terrain.Draw(batch, gameTime);

        foreach (ScatteredPart part in ScatteredParts)
        {
            part.Draw(gameTime, batch);
        }

        Garage.Draw(batch, gameTime);

        TowerManager.Draw(batch, gameTime);
        Player.Draw(gameTime, batch);
    }
}
