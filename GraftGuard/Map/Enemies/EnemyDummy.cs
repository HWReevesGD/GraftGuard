using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard.Map.Enemies;
internal class EnemyDummy : Enemy
{
    public EnemyDummy(Vector2 position)
        : base(position, hitboxSize: new Vector2(32, 48), Placeholders.TextureEnemyDummy, 4.0f, 1.0f)
    {

    }
}
