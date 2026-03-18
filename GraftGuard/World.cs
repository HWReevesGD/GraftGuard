using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard;
internal class World
{
    // Fields
    private List<PathNode> pathNodes;
    private List<Enemy> enemies;
    private Player player;

    // Properties
    public TowerManager TowerManager { get; set; }
    public TowerGrafter TowerGrafter { get; set; }
    public Terrain Terrain { get; set; }
    public Camera Camera { get; set; }
    public List<ScatteredPart> ScatteredParts { get; set; }

    // Constructor
    public World()
    {
        pathNodes = new List<PathNode>();
        enemies = new List<Enemy>();

        // These parts are just for testing, this will normally start empty
        ScatteredParts = [
            new ScatteredPart(new Vector2(420, 420), PartRegistry.GetByName("arm")),
            ];
        
        TowerManager = new TowerManager();
        TowerGrafter = new TowerGrafter(TowerManager);
        Terrain = new Terrain();

        player = new Player(Vector2.Zero);
        Camera = new Camera();
    }

    // Methods
    public void Update(GameTime gameTime, InputManager inputManager, TimeState state)
    {
        player.Update(gameTime, inputManager, this);
        inputManager.Update(Camera);
        TowerManager.Update(gameTime);
        Terrain.Update(gameTime);

        if (state == TimeState.Day)
        {
            TowerGrafter.Update(gameTime, inputManager);
        }
    }

    public void DrawStatic(SpriteBatch batch, GameTime gameTime, TimeState state)
    {
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

        TowerManager.Draw(batch, gameTime);
        player.Draw(gameTime, batch);
    }
}
