using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard
{
    internal class GameManager
    {
        private readonly World _world;
        private readonly MainMenu _mainMenu;
        private readonly PauseMenu _pauseMenu;
        private readonly TowerGraftingGUI _towerGrafting;
        private readonly InputManager inputManager;

        private const float DawnTimeLength = 10f;
        private const float NightTimeLength = 5f;

        public GameManager(World world, MainMenu menu, PauseMenu pause, TowerGraftingGUI gui, InputManager input)
        {
            _world = world;
            _mainMenu = menu;
            _pauseMenu = pause;
            _towerGrafting = gui;
            inputManager = input;

            _towerGrafting.OnNightButtonPressed += HandleStartNight;
        }

        public void Update(GameTime gameTime)
        {
            inputManager.Update(_world.Camera); //bad practice but it works

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
            }
        }

        private void UpdateMainMenu(GameTime gameTime)
        {
            if (inputManager.WasKeyPressStarted(Keys.Enter))
                PlayerData.StartNewGame(DawnTimeLength);

            _mainMenu.Update(gameTime);
        }

        private void UpdateGameplay(GameTime gameTime)
        {
            var session = PlayerData.CurrentGame;

            //Pausing
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
                session.Timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (session.Timer <= 0) HandleTimeTransition(session);
            }
        }

        private void HandleTimeTransition(GameData session)
        {
            if (session.Time == TimeState.Night)
            {
                //Game Over
                PlayerData.CurrentState = GameState.Paused;
            }
            else if (session.Time == TimeState.Dawn)
            {
                session.Time = TimeState.Day;
            }
        }

        private void HandleStartNight()
        {
            if (PlayerData.CurrentGame.Time == TimeState.Day)
            {
                PlayerData.CurrentGame.Time = TimeState.Night;
                PlayerData.CurrentGame.Timer = NightTimeLength; 
            }
        }

        private void UpdatePaused()
        {
            if (inputManager.WasKeyPressStarted(Keys.Enter))
                PlayerData.CurrentState = GameState.Game;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            switch (PlayerData.CurrentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(spriteBatch, gameTime);
                    break;
                case GameState.Game:
                    DrawGameSession(spriteBatch, gameTime);
                    break;
                case GameState.Paused:
                    _pauseMenu.Draw(spriteBatch, gameTime, PlayerData.CurrentGame.Time);
                    break;
            }
        }

        private void DrawGameSession(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var session = PlayerData.CurrentGame;

            // World Draw (Camera space)
            spriteBatch.End();
            spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _world.Camera.WorldToScreen);
            _world.DrawCamera(spriteBatch, gameTime, session.Time, inputManager, true);
            spriteBatch.End();

            // UI Draw (Screen space)
            spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            if (session.Time == TimeState.Day) _towerGrafting.Draw(spriteBatch, gameTime);

            // HUD
            //spriteBatch.DrawString(Fonts.Arial, $"TIME: {session.Time}\nTIMER: {session.Timer:F1}", new Vector2(64, 0), Color.White);
        }
    }
}
