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
        public TimeState Time { get; set; }
        public float Timer { get; set; }
        public int CurrentScore { get; set; }

        public Difficulty CurrentDifficulty { get; set; }

    }
}
