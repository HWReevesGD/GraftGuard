using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.UI;
using GraftGuard.UI.Screens;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Reflection;

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
        private readonly SwipeTransition _swipeTransition;
        private readonly PointersUI _pointers;
        private readonly NextDayTransition _nextDayTransition;
        private readonly InputManager inputManager;

        public readonly DrawManager DrawManager;

        public static readonly float DawnTimeLength = 12f;
        public static readonly float NightTimeLength = 80f;

        private GameTime lastGameTime;
        public GameManager(Game1 game, InputManager input, SpriteBatch batch)
        {
            DrawManager = new DrawManager(batch);

            _mainMenu = new MainMenu(game, input, DrawManager);
            _world = new World(DrawManager);
            _pauseMenu = new PauseMenu(_world, input);
            _gameOverScreen = new GameOverScreen(_world);
            _hud = new GameHUD();
            _towerGrafting = new TowerGraftingGUI();
            _nightPlacement = new NightPlacementGUI();
            _swipeTransition = new SwipeTransition(true);
            _pointers = new PointersUI();
            _nextDayTransition = new NextDayTransition();
            inputManager = input;

            _mainMenu.NewGameStarted += OnNewGameStarted;

            _towerGrafting.OnNightButtonPressed += OnStartingNight;
        }

        private void HandleDeath()
        {
            if (PlayerData.CurrentState == GameState.Game)
            {
                ToggleGameOver(lastGameTime, PlayerData.CurrentGame, "You Died");
            }

        }

        public void Update(GameTime gameTime)
        {
            inputManager.Update(_world.Camera);
            TaskSchedule.UpdateAll(gameTime);

            if (inputManager.WasKeyPressStarted(Keys.T))
            {
                ToggleGameOver(lastGameTime, PlayerData.CurrentGame, "You pressed the die key");
            }

            switch (PlayerData.CurrentState)
            {
                case GameState.MainMenu:
                    UpdateMainMenu(gameTime);
                    _nextDayTransition.Reset();
                    break;
                case GameState.Game:
                    if (!PlayerData.CurrentGame.PauseForTimeTransitioning)
                    {
                        UpdateGameplay(gameTime);
                    }
                    else
                    {
                        _nextDayTransition.Update(gameTime);
                    }
                    break;
                case GameState.Paused:
                    UpdatePaused();
                    break;
                case GameState.GameOver:
                    UpdateGameOver(gameTime);
                    _nextDayTransition.Reset();
                    break;
            }

            lastGameTime = gameTime;
        }
        private void OnNewGameStarted()
        {
            PlayerData.CurrentGame.PauseForTimeTransitioning = false;
            _world.OnNewGameStarted();
            _nextDayTransition.Reset();
            PlayerData.CurrentGame = new();
            OnStartingDawn(isFirstDawn: true);

            PlayerData.CurrentGame.OnPlayerDied += HandleDeath;
            _swipeTransition.Start(lastGameTime, true);
        }

        /// <summary>
        /// Runs when Dawn is started
        /// </summary>
        private void OnStartingDawn(bool isFirstDawn = false)
        {
            if (!isFirstDawn)
            {
                PlayerData.CurrentGame.GameLog.RoundsSurvived++;
                _nextDayTransition.Reset();
                PlayerData.CurrentGame.PauseForTimeTransitioning = true;
            }
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
            // Setup Player Data
            PlayerData.CurrentGame.Time = TimeState.Night;
            PlayerData.CurrentGame.PhaseTimeLength = NightTimeLength;

            _world.OnStartingNight();
            PlayerData.CurrentGame.Timer = _world.EnemyManager.WaveManager.FullTime;

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

                //if (session.Time == TimeState.Night && ((_world.EnemyManager.Enemies.Count == 0 || _world.EnemyManager.Enemies.All((enemy) => enemy.IsDead)) && _world.EnemyManager.WaveManager.AllWavesStarted))
                //{
                //    HandleTimeTransition(gameTime, session);
                //}
                //else
                if (session.Timer <= 0)
                {
                    HandleTimeTransition(gameTime, session);
                }
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

            if (_world.EnemyManager.Enemies.Any((enemy) => !enemy.IsDead && enemy.Hitbox.Intersects(_world.Garage.GameOverBounds)))
            {
                ToggleGameOver(gameTime, PlayerData.CurrentGame, "Your Garage was Invaded");
            }
        }

        private void HandleTimeTransition(GameTime gameTime, GameData session)
        {
            switch (session.Time)
            {
                case TimeState.Night:
                    session.Timer = DawnTimeLength;
                    session.Time = TimeState.Dawn;
                    _world.EnemyManager.Enemies.Clear();
                    OnStartingDawn();
                    break;
                case TimeState.Dawn:
                    session.Time = TimeState.Day;
                    OnStartingDay();
                    break;
            }
        }

        private void ToggleGameOver(GameTime gameTime, GameData session, string failReason)
        {
            PlayerData.CurrentState = GameState.GameOver;
            _gameOverScreen.SetGameOver(gameTime, session, failReason);
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
                    _nextDayTransition.Draw(DrawManager, gameTime);

                    break;
                case GameState.Paused:
                    _pauseMenu.Draw(DrawManager, gameTime, PlayerData.CurrentGame.Time);
                    break;
                case GameState.GameOver:
                    _gameOverScreen.Draw(DrawManager, gameTime);
                    break;
            }
            DrawManager.Paint(_world.Camera);
            //TaskSchedule.DrawDebug(DrawManager);
        }

        private void DrawGameSession(DrawManager drawing, GameTime gameTime)
        {
            var session = PlayerData.CurrentGame;

            if (session.Time != TimeState.Day)
            {
                _world.DrawCamera(drawing, gameTime, session.Time, inputManager, true);
            }

            // Draw Time Overlay
            switch (PlayerData.CurrentGame.Time)
            {
                case TimeState.Night:
                    drawing.Draw(Placeholders.TexturePixel, destination: new Rectangle(Point.Zero, Interface.ScreenSize.ToPoint()), color: new Color(0.1f, 0f, 0.4f, 0.1f), isUi: true);
                    break;
                case TimeState.Dawn:
                    drawing.Draw(Placeholders.TexturePixel, destination: new Rectangle(Point.Zero, Interface.ScreenSize.ToPoint()), color: new Color(0.8f, 0.6f, 0.3f, 0.01f), isUi: true);
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

            _pointers.Draw(DrawManager, _world);
            _hud.Draw(drawing, gameTime, session.Time != TimeState.Day);
            _swipeTransition.Draw(drawing, gameTime);

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
