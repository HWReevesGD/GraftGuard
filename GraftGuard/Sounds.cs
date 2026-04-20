using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard;
internal static class Sounds
{
    private static ContentManager _content;
    public static readonly string SoundPath = "Sound/sfx_";
    public static readonly string SongPath = "Sound/music_";
    public static SoundEffect Swish { get; private set; }
    public static Song SongDawnPeaceful { get; private set; }
    public static Song SongNightEasy { get; private set; }
    public static Song SongDayA { get; private set; }
    public static void LoadContent(ContentManager content)
    {
        _content = content;

        Swish = Sound("swishing");
        SongDawnPeaceful = Song("dawn_peaceful");
        SongNightEasy = Song("night_easy");
        SongDayA = Song("day_1");
    }

    private static SoundEffect Sound(string soundName)
    {
        return _content.Load<SoundEffect>(SoundPath + soundName);
    }
    private static Song Song(string soundName)
    {
        return _content.Load<Song>(SongPath + soundName);
    }
}
