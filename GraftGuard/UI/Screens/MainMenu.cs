// render main menu

using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.Graphics.Particles;
using GraftGuard.Graphics.TextEffects;
using GraftGuard.Graphics.TextEffects.Effects;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Pathing;
using GraftGuard.Map.Waves;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GraftGuard.UI.Screens;

internal delegate void ActionEvent();

internal class MainMenu {
    private readonly World backgroundWorld;
    private readonly InputManager idleInputManager; // input manager that will never update, because it's for the background (world needs it)
    private readonly InputManager inputManager;
    private readonly TimeState timeState;

    private readonly static float cameraPanSpeed = 0.5f;

    private readonly static float titleLeftPadding = 20;
    private readonly static float itemLeftPadding = 40;
    private readonly static float itemBottomPadding = 20;
    private readonly static float itemGap = 10;
    private readonly static float itemShakeIntensity = 3;
    private readonly static float selectedItemLeftOffset = 34;
    private readonly static float lerpSpeed = 15;

    private readonly static float gameBeginMenuItemsLeftPosition = -1500;
    private readonly static float gameBeginMenuitemsEaseTime = 0.25f;
    private readonly static float gameBeginDelayTime = 0.5f;

    private static Texture2D backgroundTexture;
    private readonly static string titleText = "GRAFTGUARD";

    // percentage range for path length from a spawn to the garage
    // (min 0.2f = spawn minimum 20% of the way from a spawn to the garage)
    private readonly static float towerSpawnPathLengthMinPercentage = 0.25f;
    private readonly static float towerSpawnPathLengthMaxPercentage = 0.75f;
    private readonly static float towerSpawnMaxRandomOffset = 75f;
    private readonly static int amountOfTowers = 15;

    private static readonly float minEnemySpawnPeriod = 1f;
    private static readonly int maxEnemyCount = 50;
    private static readonly float timeScale = 0.8f;

    // automatic camera panning cycle stuff
    private static readonly float xCycleOffset = 150;
    private static readonly float yCycleOffset = -300;
    private static readonly float xCycleScale = 0.25f;
    private static readonly float yCycleScale = 0.25f;
    private static readonly float manualCameraPanSpeed = 500;

    private static readonly string[] menuItemOrder = [
        "Start Game",
        "Continue Game",
        "Options",
        "View Componedium"
        ];

    public static void LoadContent(ContentManager content)
    {
        backgroundTexture = content.Load<Texture2D>("pixel");
    }

    private Game1 game;
    private int selectedItemIndex;
    private float[] itemXOffests;
    private float[] itemWaveAmplitudes;
    private float arrowYPosition;

    private SwipeTransition swipeTransition;
    private bool optionWasPicked;
    private float optionPickedTime;

    public event ActionEvent NewGameStarted;

    private Random rng;
    private bool manualPanningEnabled;
    private float x;
    private float y;
    private float lastEnemySpawnTime;

    public MainMenu(Game1 game, InputManager inputManager, DrawManager drawing)
    {
        this.game = game;
        this.backgroundWorld = new World(drawing);
        this.idleInputManager = new InputManager();
        this.inputManager = inputManager;
        this.timeState = TimeState.Night; // always night
        this.selectedItemIndex = 0;
        this.itemXOffests = new float[menuItemOrder.Length];
        this.itemWaveAmplitudes = new float[menuItemOrder.Length];
        this.arrowYPosition = Interface.Height;

        swipeTransition = new SwipeTransition(false);

        rng = new Random();

        // move player into the garage so player-targetting enemies go to it
        backgroundWorld.Player.Position = backgroundWorld.Garage.Center;
        backgroundWorld.ShowTowerDecay = false;

        CreateTowers(amountOfTowers);
    }

    /// <summary>
    /// Get a random point where a tower could be spawned
    /// </summary>
    /// <returns>Point</returns>
    private Vector2 GetRandomTowerSpawnPosition()
    {
        Vector2 enemySpawn = EnvironmentRegistry.Map.EnemySpawns[rng.Next(0, EnvironmentRegistry.Map.EnemySpawns.Count)];

        List<PathNode> path = backgroundWorld.EnemyManager.PathManager.FindPath(
                backgroundWorld,
                enemySpawn,
                new PathManager.PathSettings() { Goal = PathManager.PathGoal.Garage }
                );

        float pathSpawnDistancePercentage = MathHelper.Lerp(
            towerSpawnPathLengthMinPercentage,
            towerSpawnPathLengthMaxPercentage,
            (float)rng.NextDouble()
            );

        Vector2 nodePosition = path[(int)(path.Count * pathSpawnDistancePercentage)].WorldPosition;
        return new Vector2(
            nodePosition.X - towerSpawnMaxRandomOffset / 2 + (float)rng.NextDouble() * towerSpawnMaxRandomOffset,
            nodePosition.Y - towerSpawnMaxRandomOffset / 2 + (float)rng.NextDouble() * towerSpawnMaxRandomOffset
            );
    }

    /// <summary>
    /// Create an amount of random towers positioned on existing paths from enemy spawns to the garage
    /// </summary>
    /// <param name="amount">Amount of towers to spawn</param>
    private void CreateTowers(int amount)
    {
        // create random towers for the world

        for (int _ = 0; _ < amount; _++)
        {
            // pick random tower type and random parts
            TowerDefinition tower = TowerRegistry.Towers[rng.Next(0, TowerRegistry.Towers.Count)];
            List<PartDefinition> parts = new List<PartDefinition>();
            Vector2 position = GetRandomTowerSpawnPosition();

            // at least one part
            for (int i = 0; i < rng.Next(1, 4); i++)
                parts.Add(GraftLibrary.GetRandomPart());

            // MakeTower() will update path costs, so there will automatically not be
            // towers overlapping on the main menu
            backgroundWorld.TowerManager.MakeTower(tower, position, parts);
        }
    }

    /// <summary>
    /// Update bmain menu
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void Update(GameTime gameTime)
    {
        if (inputManager.WasKeyPressStarted(Keys.Escape))
            game.Exit();

        UpdateBackgroundWorld(gameTime);
        UpdateMenu(gameTime);
    }

    /// <summary>
    /// Automatic panning around the map
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    private void UpdateCameraAutoPanning(GameTime gameTime)
    {
        MapDefinition map = EnvironmentRegistry.Map;
        Rectangle pathingArea = map.PathingArea;
        int width = pathingArea.Width;
        int height = pathingArea.Height;

        float cycleWidth = width * xCycleScale;
        float cycleHeight = height * yCycleScale;

        double xCycle = Math.Cos(gameTime.TotalGameTime.TotalSeconds / 3 * cameraPanSpeed);
        double yCycle = Math.Sin(gameTime.TotalGameTime.TotalSeconds * cameraPanSpeed);

        x = (float)(xCycle * cycleWidth / 2 + pathingArea.Center.X) + xCycleOffset;
        y = (float)(yCycle * cycleHeight / 2 + pathingArea.Center.Y) + yCycleOffset;
    } 

    /// <summary>
    /// Panning controlled by WASD
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    public void UpdateCameraManualPanning(GameTime gameTime)
    {
        float delta = gameTime.Delta();
        if (inputManager.IsKeyDown(Keys.W))
            y -= manualCameraPanSpeed * delta;
        if (inputManager.IsKeyDown(Keys.A))
            x -= manualCameraPanSpeed * delta;
        if (inputManager.IsKeyDown(Keys.S))
            y += manualCameraPanSpeed * delta;
        if (inputManager.IsKeyDown(Keys.D))
            x += manualCameraPanSpeed * delta;
    }

    /// <summary>
    /// Update background world simulation and pan the camera
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void UpdateBackgroundWorld(GameTime gameTime)
    {
        // update world

        // spawn enemies in interval if the amount is below the maximum
        if (gameTime.TotalGameTime.TotalSeconds - lastEnemySpawnTime > minEnemySpawnPeriod)
        {
            lastEnemySpawnTime = (float)gameTime.TotalGameTime.TotalSeconds;
            if (backgroundWorld.EnemyManager.Enemies.Count < maxEnemyCount)
            {
                // yes it is just spawning the wave for round 0 that is intentional
                NightWaveSet waveSet = WavesRegistry.GetRandomForRound(0);
                foreach (NightWave wave in waveSet.Waves)
                    backgroundWorld.EnemyManager.SpawnWave(wave);
            }
        }

        backgroundWorld.Update(gameTime.Scale(timeScale), idleInputManager, timeState, false);

        // remove enemies that wouldve killed the player via garage
        for (int i = 0; i < backgroundWorld.EnemyManager.Enemies.Count; i++)
        {
            Enemy enemy = backgroundWorld.EnemyManager.Enemies[i];
            if (!enemy.IsDead && enemy.Hitbox.Intersects(backgroundWorld.Garage.GameOverBounds))
            {
                backgroundWorld.EnemyManager.Enemies.RemoveAt(i);
                i--;
            }
        }

        // camera

        if (manualPanningEnabled)
            UpdateCameraManualPanning(gameTime);
        else
            UpdateCameraAutoPanning(gameTime);

        backgroundWorld.Camera.Position = new Vector2(x, y);

        // debug keys

        // spawn wave of enemies
        if (inputManager.WasKeyPressStarted(Keys.Y))
        {
            NightWaveSet waveSet = WavesRegistry.GetRandomForRound(1);
            foreach (NightWave wave in waveSet.Waves)
                backgroundWorld.EnemyManager.SpawnWave(wave);
        }

        // make a tower
        if (inputManager.WasKeyPressStarted(Keys.U))
            CreateTowers(1);

        // toggle manual camera panning
        if (inputManager.WasKeyPressStarted(Keys.P))
            manualPanningEnabled = !manualPanningEnabled;
    }

    /// <summary>
    /// Process key inputs for regular stuff (NOT DEBUG KEYS!)
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    public void ProcessKeys(GameTime gameTime)
    {
        if (inputManager.WasKeyPressStarted(Keys.Up)) // advance back
            selectedItemIndex = (selectedItemIndex + menuItemOrder.Length - 1) % menuItemOrder.Length;
        if (inputManager.WasKeyPressStarted(Keys.Down)) // advance forward
            selectedItemIndex = (selectedItemIndex + 1) % menuItemOrder.Length;

        if (inputManager.WasKeyPressStarted(Keys.Enter))
        {
            switch (selectedItemIndex)
            {
                case 0: // start new game
                    optionWasPicked = true;
                    optionPickedTime = (float)gameTime.TotalGameTime.TotalSeconds;
                    swipeTransition.Start(gameTime, false);

                    new TaskSchedule()
                        .Wait(gameBeginDelayTime)
                        .Run(() => {
                            PlayerData.StartNewGame(GameManager.DawnTimeLength);
                            NewGameStarted?.Invoke();
                            swipeTransition.Clear();
                            // reset main menu
                            optionWasPicked = false;
                        });
                    break;
                case 1: // continue game
                    break;
                case 2: // options
                    break;
                case 3: // compendium
                    break;
            }
        }
    }

    /// <summary>
    /// Update main menu by input requests
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void UpdateMenu(GameTime gameTime)
    {
        if (!optionWasPicked)
            ProcessKeys(gameTime);

        inputManager.Update();
    }

    /// <summary>
    /// Draw Main Menu
    /// </summary>
    /// <param name="drawing">SpriteBatch</param>
    /// <param name="gameTime">gameTime from Game1 Draw()</param>
    public void Draw(DrawManager drawing, GameTime gameTime)
    {
        // simulate and render world in the background
        // with random towers and constantly spawning enemies
        // if the world succumbs to an enemy, don't

        // draw background world

        drawing.ExtraMatrix = backgroundWorld.Camera.WorldToScreen * Matrix.CreateScale(0.25f);
        // Draw by the Camera's Position
        backgroundWorld.DrawCamera(drawing, gameTime.Scale(timeScale), timeState, inputManager, false);
        drawing.ExtraMatrix = null;

        // draw transulcent black cover

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        Color bgColor = new Color(0, 0, 0, 0.75f);
        drawing.Draw(backgroundTexture, fullScreenRect, color: bgColor, isUi: true);

        // draw menu items

        float alpha = Math.Min(gameTime.Delta() * lerpSpeed, 1);
        float yPosition = Interface.Height - itemBottomPadding;

        float unpickedXOffsetPosition = 0;
        if (optionWasPicked)
        {
            float elapsed = (float)gameTime.TotalGameTime.TotalSeconds - optionPickedTime;
            float prog = Math.Min(elapsed / gameBeginMenuitemsEaseTime, 1);
            unpickedXOffsetPosition = gameBeginMenuItemsLeftPosition * (float)Math.Pow(prog, 2);
        }

        // draw each menu item

        bool pickedItemShouldDraw = !optionWasPicked || Math.Round(gameTime.TotalGameTime.TotalSeconds / 0.05f) % 2 == 0;

        for (int i = menuItemOrder.Length - 1; i >= 0; i--)
        {
            // update effects for this item

            float targetXOffset;
            float targetWaveAmplitude;
            bool shouldDraw = true;

            if (i == selectedItemIndex)
            {
                targetXOffset = selectedItemLeftOffset;
                targetWaveAmplitude = itemShakeIntensity;
                arrowYPosition = MathHelper.Lerp(arrowYPosition, yPosition, alpha);
                shouldDraw = pickedItemShouldDraw;
            }
            else
            {
                targetXOffset = unpickedXOffsetPosition;
                targetWaveAmplitude = 0;
            }

            itemXOffests[i] = MathHelper.Lerp(itemXOffests[i], targetXOffset, alpha);
            itemWaveAmplitudes[i] = MathHelper.Lerp(itemWaveAmplitudes[i], targetWaveAmplitude, alpha);

            // render item on the bottom left yea

            Text text = new Text(Fonts.SubFont, menuItemOrder[i])
                    .SetYOrigin(YOrigin.Bottom)
                    .SetKerning(2)
                    .AddEffect(new ShakeTextEffect(itemWaveAmplitudes[i]));

            if (shouldDraw)
                text.Draw(drawing, gameTime, new Vector2(itemLeftPadding + itemXOffests[i], yPosition));

            // increment up
            yPosition += -text.Height - itemGap;
        }

        // little arrow thing
        if (pickedItemShouldDraw)
            new Text(Fonts.SubFont, ">")
                .SetYOrigin(YOrigin.Bottom)
                .DrawRaw(drawing, new Vector2(itemLeftPadding, arrowYPosition));

        // title text
        new Text(Fonts.MainFont, titleText)
            .SetYOrigin(YOrigin.Bottom)
            .SetKerning(3)
            .AddEffect(new WavyTextEffect(7 + unpickedXOffsetPosition, -3))
            .Draw(drawing, gameTime, new Vector2(titleLeftPadding + unpickedXOffsetPosition, yPosition));

        swipeTransition.Draw(drawing, gameTime);
    }
}