using GraftGuard.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GraftGuard.Map
{

    public enum PropType
    {
        LargeTree1,
        LargeTree2,
        BushSpiky,
        BushRound,
        Flowers1,
        Flowers2,
        Flowers3,
        SmallBush
    }

    public static class EnvironmentProps
    {
        private static Texture2D sheet;
        private static Dictionary<PropType, Rectangle> sources;

        public static float GlobalScale = 2.0f;

        public static void LoadContent(ContentManager content)
        {
            sheet = content.Load<Texture2D>("Environment/trees-and-bushes_transparent");
            sources = new Dictionary<PropType, Rectangle>
            {
                { PropType.LargeTree1, new Rectangle(0, 7, 113, 118) },
                { PropType.LargeTree2, new Rectangle(120, 7, 102, 140) },
                { PropType.BushSpiky, new Rectangle(224, 6, 30, 39) },
                { PropType.BushRound, new Rectangle(261, 1, 25, 30) },
                { PropType.Flowers1, new Rectangle(227, 55, 21, 19) },
                { PropType.Flowers2, new Rectangle(255, 50, 15, 13) },
                { PropType.Flowers3, new Rectangle(228, 88, 19, 18) },
                { PropType.SmallBush, new Rectangle(240, 116, 32, 37) }
            };
        }

        internal static void Draw(DrawManager drawing, PropType type, Vector2 position)
        {
            if (sources.TryGetValue(type, out Rectangle sourceRect))
            {
                //base of tree/bush instead of top left
                Vector2 origin = new Vector2(sourceRect.Width / 2f, sourceRect.Height);

                drawing.Draw(
                    texture: sheet,
                    position: position,
                    source: sourceRect,
                    origin: origin,       
                    scale: GlobalScale * Vector2.One    
                );
            }
        }
    }
}
