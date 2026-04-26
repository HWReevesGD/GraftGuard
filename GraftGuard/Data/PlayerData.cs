using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace GraftGuard.Data
{
    /// <summary>
    /// The Save Data Structure for JSON Serialization
    /// </summary>
    internal class SaveData
    {
        public int HighScore { get; set; }
        public List<GameData> GameHistory { get; set; }

    }

    internal static class PlayerData
    {
        public static GameState CurrentState { get; set; } = GameState.MainMenu;

        public static int HighScore { get; set; } = 0;
        public static List<GameData> GameHistory { get; set; } = new List<GameData>();


        // The active game
        public static GameData CurrentGame { get; set; } = new GameData();

        private static string SaveFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save.json");

        public static void StartNewGame(float initialTimer)
        {
            CurrentGame = new GameData
            {
                Time = TimeState.Dawn,
                Timer = initialTimer
            };
            CurrentState = GameState.Game;
        }

        public static void RecordCurrentGame()
        {
            GameHistory ??= new List<GameData>();
            GameHistory.Add(CurrentGame);

            Debug.WriteLine($"{CurrentGame.CurrentScore} > {HighScore} = {CurrentGame.CurrentScore > HighScore}");
            if (CurrentGame.CurrentScore > HighScore)
            {
                HighScore = CurrentGame.CurrentScore;
            }
        }

        //for future
        public static void Save()
        {
            var data = new SaveData
            {
                HighScore = HighScore,
                GameHistory = GameHistory
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFilePath, json);
        }

        public static void Load()
        {
            if (!File.Exists(SaveFilePath)) return;

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var data = JsonSerializer.Deserialize<SaveData>(json);

                if (data != null)
                {
                    HighScore = data.HighScore;
                    GameHistory = data.GameHistory ?? new List<GameData>();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load save: {e.Message}");
            }
        }
    

    
    }
}
