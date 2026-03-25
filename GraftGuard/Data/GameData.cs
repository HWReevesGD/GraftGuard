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
    public class GameData
    {
        public TimeState Time { get; set; }
        public float Timer { get; set; }
        public int CurrentScore { get; set; }

    }
}
