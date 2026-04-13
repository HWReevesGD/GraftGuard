using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.UI;
using GraftGuard.UI.Screens;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GraftGuard
{
    internal class GameManager
    {
        private readonly World _world;
        private readonly MainMenu _mainMenu;
        private readonly PauseMenu _pauseMenu;
        private readonly GameOverScreen _gameOverScreen;
        private readonly GameHUD _hud;
        private readonly TowerGraftingGUI _towerGrafting;
        private readonly NightPlacementGUI _nightPlacement;
        private readonly InputManager inputManager;

        public readonly DrawManager DrawManager;

        public static readonly float DawnTimeLength = 10f;
        public static readonly float NightTimeLength = 20f;

        private GameTime lastGameTime;
        public GameManager(Game1 game, InputManager input, SpriteBatch batch)
        {
            DrawManager = new DrawManager(batch);

            _world = new World(DrawManager);
            _mainMenu = new MainMenu(game, input, DrawManager);
            _pauseMenu = new PauseMenu(_world, input);
            _gameOverScreen = new GameOverScreen(_world);
            _hud = new GameHUD();
            _towerGrafting = new TowerGraftingGUI();
            _nightPlacement = new NightPlacementGUI();
            inputManager = input;

            _mainMenu.NewGameStarted += OnNewGameStarted;

            _towerGrafting.OnNightButtonPressed += OnStartingNight;
        }

        private void HandleDeath()
        {
            if (PlayerData.CurrentState == GameState.Game)
            {
                ToggleGameOver(lastGameTime, PlayerData.CurrentGame);
            }

        }

        public void Update(GameTime gameTime)
        {
            inputManager.Update(_world.Camera); 

            switch (PlayerData.CurrentState)
            {
                case GameState.MainMenu:
                    UpdateMainMenu(gameTime);
                    break;
                case GameState.Game:
                    UpdateGameplay(gameTime);
                    break;
                case GameState.Paused:
                    UpdatePaused();
                    break;
                case GameState.GameOver:
                    UpdateGameOver(gameTime);
                    break;
            }

            lastGameTime = gameTime;
        }
        private void OnNewGameStarted()
        {
            _world.OnNewGameStarted();
            PlayerData.CurrentGame = new();
            OnStartingDawn();

            PlayerData.CurrentGame.OnPlayerDied += HandleDeath;
        }

        /// <summary>
        /// Runs when Dawn is started
        /// </summary>
        private void OnStartingDawn()
        {
            PlayerData.CurrentGame.Timer = DawnTimeLength;
            _world.OnStartingDawn();
        }

        /// <summary>
        /// Runs when Day is started
        /// </summary>
        private void OnStartingDay()
        {
            _world.OnStartingDay();
            _towerGrafting.Setup(_world.Inventory);
        }

        /// <summary>
        /// Runs when Night is started
        /// </summary>
        private void OnStartingNight()
        {
            PlayerData.CurrentGame.Timer = NightTimeLength;
            // Setup Player Data
            if (PlayerData.CurrentGame.Time == TimeState.Day)
            {
                PlayerData.CurrentGame.Time = TimeState.Night;
                PlayerData.CurrentGame.PhaseTimeLength = NightTimeLength;
                PlayerData.CurrentGame.Timer = NightTimeLength;
            }

            _world.OnStartingNight();

            // Setup Night Placement GUI
            _nightPlacement.Setup(_world.Inventory);
        }

        private void UpdateMainMenu(GameTime gameTime)
        {
            _mainMenu.Update(gameTime);
        }

        private void UpdateGameplay(GameTime gameTime)
        {
            var session = PlayerData.CurrentGame;

            // Pausing
            if (inputManager.WasKeyPressStarted(Keys.Escape))
            {
                PlayerData.CurrentState = GameState.Paused;
                return;
            }

            //bool canPlayerMove = (session.Time != TimeState.Day);

            _world.Update(gameTime, inputManager, session.Time, true);

            if (session.Time == TimeState.Day)
            {
                _towerGrafting.Update(gameTime, inputManager, _world);
            }
            else
            {
                session.Timer -= gameTime.Delta();
                if (session.Timer <= 0) HandleTimeTransition(gameTime, session);
            }

            switch (session.Time)
            {
                case TimeState.Night:
                    UpdateNightOnly(gameTime);
                    break;
                case TimeState.Dawn:
                    break;
                case TimeState.Day:
                    break;
            }
        }

        /// <summary>
        /// Updates systems which only should be updated during Night
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        private void UpdateNightOnly(GameTime gameTime)
        {
            _nightPlacement.Update(gameTime, _world, inputManager);
        }

        private void HandleTimeTransition(GameTime gameTime, GameData session)
        {
            //if time ran out at night it's game over for now probably rethink this
            if (session.Time == TimeState.Night)
            {
                //Game Over
                ToggleGameOver(gameTime, session);
            }
            else if (session.Time == TimeState.Dawn)
            {
                session.Time = TimeState.Day;
                OnStartingDay();
            }
        }

        private void ToggleGameOver(GameTime gameTime, GameData session)
        {
            PlayerData.CurrentState = GameState.GameOver;
            _gameOverScreen.SetSession(gameTime, session);
        }

        private void UpdatePaused()
        {
            _pauseMenu.Update();
        }

        private void UpdateGameOver(GameTime gameTime)
        {
            _gameOverScreen.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            switch (PlayerData.CurrentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(DrawManager, gameTime);
                    break;
                case GameState.Game:
                    DrawGameSession(DrawManager, gameTime);
                    break;
                case GameState.Paused:
                    _pauseMenu.Draw(DrawManager, gameTime, PlayerData.CurrentGame.Time);
                    break;
                case GameState.GameOver:
                    _gameOverScreen.Draw(DrawManager, gameTime);
                    break;
            }
            DrawManager.Paint(_world.Camera);
        }

        private void DrawGameSession(DrawManager drawing, GameTime gameTime)
        {
            var session = PlayerData.CurrentGame;

            _world.DrawCamera(drawing, gameTime, session.Time, inputManager, true);

            // Draw Time Overlay
            switch (PlayerData.CurrentGame.Time)
            {
                case TimeState.Night:
                    drawing.Draw(Placeholders.TexturePixel, destination: new Rectangle(Point.Zero, Interface.ScreenSize.ToPoint()), color: new Color(0.1f, 0f, 0.4f, 0.1f));
                    break;
                case TimeState.Dawn:
                    drawing.Draw(Placeholders.TexturePixel, destination: new Rectangle(Point.Zero, Interface.ScreenSize.ToPoint()), color: new Color(0.8f, 0.6f, 0.3f, 0.01f));
                    break;
                case TimeState.Day:
                    break;
            }

            // UI Draw (Screen space)
            switch (session.Time)
            {
                case TimeState.Dawn:
                    break;
                case TimeState.Day:
                    _towerGrafting.Draw(drawing, gameTime, _world, inputManager);
                    break;
                case TimeState.Night:
                    DrawNightOnly(drawing, gameTime);
                    break;
            }

            _hud.Draw(drawing, gameTime, session.Time != TimeState.Day);

            // HUD
            //spriteBatch.DrawString(Fonts.Arial, $"TIME: {session.Time}\nTIMER: {session.Timer:F1}", new Vector2(64, 0), Color.White);
        }

        /// <summary>
        /// Draws systems which only should be drawing during Nighttime
        /// </summary>
        /// <param name="drawing">Batch to use</param>
        /// <param name="gameTime">Game Time to use</param>
        private void DrawNightOnly(DrawManager drawing, GameTime gameTime)
        {
            _nightPlacement.Draw(drawing, gameTime, _world, inputManager);
        }
    }
}
