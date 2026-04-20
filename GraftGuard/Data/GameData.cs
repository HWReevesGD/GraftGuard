using GraftGuard.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Data
{
    public enum GameState
    {
        MainMenu,
        Paused,
        GameOver,
        Game
    }

    public enum TimeState
    {
        Night,
        Dawn,
        Day
    }

    public enum Difficulty
    {
        Easy = 1,
        Medium = 2,
        Hard = 3,
    }

    public class GameLog
    {
        public int PartsCollected { get; set; }
        public int RoundsSurvived { get; set; }
        public int TowersMade { get; set; }
        public int EnemiesKilled { get; set; }

        public GameLog() { 
            PartsCollected = 0;
            RoundsSurvived = 0;
            EnemiesKilled = 0;
            TowersMade = 0;
        }

    }

    public class GameData
    {
        public event Action OnPlayerDied;
        private int _health;

        public TimeState Time { get; set; }
        public float Timer { get; set; }
        public float PhaseTimeLength { get; set; }

        public GameLog GameLog { get; set; }
        public int CurrentRound { get; set; }
        public bool PauseForTimeTransitioning { get; set; } = false;

        public int CurrentScore => GameLog.PartsCollected + 2 * GameLog.TowersMade + GameLog.EnemiesKilled * 3 + GameLog.RoundsSurvived;

        public int Health
        {
            get => _health;
            set
            {
                _health = value;
                if (_health <= 0)
                {
                    _health = 0; // Clamp it
                    OnPlayerDied?.Invoke(); // Fire the event!
                }
            }
        }

        public int MaxHealth { get; set; }

        public Difficulty CurrentDifficulty { get; set; }

        public GameData()
        {
            Time = TimeState.Dawn;
            PhaseTimeLength = 15;
            Timer = PhaseTimeLength;
            GameLog = new();
            MaxHealth = 24;
            Health = MaxHealth;
            CurrentDifficulty = Difficulty.Medium;
        }

    }
}
