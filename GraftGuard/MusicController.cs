using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard;
internal static class MusicController
{
    public static void Play(Song song, float fadeTime = 0.0f)
    {
        if (fadeTime == 0.0f)
        {
            MediaPlayer.Play(song);
        }
    }
}