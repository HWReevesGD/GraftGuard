using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Graphics.TextEffects
{
    internal interface ITextEffect
    {
        public Letter DoEffect(GameTime gameTime, Letter letter);
    }
}
