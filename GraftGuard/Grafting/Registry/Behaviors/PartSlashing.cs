using GraftGuard.Data;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry.Behaviors;
internal class PartSlashing : IPartBehavior
{
    private float _slashSize = 0.9f;
    public const float HitRadius = 14.0f;
    public static string Name => "Slashing";

    public static IPartBehavior Create() => new PartSlashing();

    public void Draw(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state)
    {
        if (_slashSize <= 0.0f)
        {
            return;
        }

        _slashSize = MathF.Max(0.0f, _slashSize - (float)time.ElapsedGameTime.TotalSeconds);

        drawing.DrawCentered(Placeholders.TextureSlash, transform.Position, rotation: (float)time.TotalGameTime.TotalSeconds * -24.0f % MathF.Tau, scale: _slashSize * Vector2.One, color: new Color(Color.White, 0.3f));
    }

    public void OnDealDamage(PartSettings settings, float damageModifier, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        _slashSize = 0.9f;
        Damage areaDamage = new Damage(part.PartDamage.BaseDamage * 0.5f, part.PartDamage.DamageOverTime, part.PartDamage.DamageOverTimeDuration, part.PartDamage.SpeedMod, part.PartDamage.SpeedModDuration);
        world.EnemyManager.DealDamageInAreas([], [new Circle(transform.Position, HitRadius)], areaDamage);
    }

    public void Update(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        
    }
}
