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

    public class GameData
    {
        public event Action OnPlayerDied;
        private int _health;

        public TimeState Time { get; set; }
        public float Timer { get; set; }
        public float PhaseTimeLength { get; set; }
        public int CurrentScore { get; set; }
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

        public Difficulty CurrentDifficulty { get; set; }

        public GameData()
        {
            Time = TimeState.Dawn;
            PhaseTimeLength = 15;
            Timer = PhaseTimeLength;
            CurrentScore = 0;
            Health = 3;
            CurrentDifficulty = Difficulty.Medium;
        }

    }
}
