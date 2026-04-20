using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI.Screens;
internal class NextDayTransition
{
    public float Time { get; private set; }
    public float TimeLeft => EndTime - Time;
    public const float EndTime = 2.0f;
    public float Alpha { get; private set; }
    private bool _displaysNextDay = false;

    public void Reset()
    {
        Time = 0.0f;
        Alpha = 0.0f;
        _displaysNextDay = false;
    }

    public void Update(GameTime time)
    {
        Time += time.Delta();

        if (Time >= EndTime)
        {
            EndTransition();
        }
    }

    public void Draw(DrawManager drawing, GameTime time)
    {
        if (Time < 1.0f)
        {
            Alpha = MathF.Min(Time, 1.0f);
        }
        else
        {
            Alpha = MathF.Min(TimeLeft, 1.0f);
        }

        drawing.Draw(Placeholders.TexturePixel, Interface.ScreenRect, drawLayer: 4, isUi: true, color: new Color(Color.Black, Alpha));

        string day = $"DAY: {PlayerData.CurrentGame.GameLog.RoundsSurvived}";

        if (!_displaysNextDay && Time > (.4f * EndTime))
        {
            _displaysNextDay = true;
            // Play cool sound
        }

        if (_displaysNextDay)
        {
            day = $"DAY: {PlayerData.CurrentGame.GameLog.RoundsSurvived + 1}";
        }

        drawing.DrawString(day, Interface.ScreenCenter, centered: true, font: Fonts.MainFont, color: new Color(Color.White, Alpha), drawLayer: 5, isUi: true);
    }

    public void EndTransition()
    {
        MusicController.Play(Sounds.SongDawnPeaceful);
        PlayerData.CurrentGame.PauseForTimeTransitioning = false;
        Reset();
    }

}
