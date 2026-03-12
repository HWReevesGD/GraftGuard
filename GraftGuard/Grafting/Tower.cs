using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting
{
    internal class Tower : GameObject
    {
        public static Texture2D TexturePlaceholder1 { get; private set; }
        public static Texture2D TexturePlaceholder2 { get; private set; }

        public static void LoadContent(ContentManager content)
        {
            TexturePlaceholder1 = content.Load<Texture2D>("Placeholder/tower_placeholder_1");
            TexturePlaceholder2 = content.Load<Texture2D>("Placeholder/tower_placeholder_2");
        }

        public enum Slot
        {
            First,
            Second,
            Third,
            Fourth,
        }

        protected PartDefinition[] _attachedParts;
        
        /// <summary>
        /// True if there is at least one non-null part attached
        /// </summary>
        public bool HasParts => _attachedParts.Any((part) => part is not null);
        public int TotalAttachedParts => _attachedParts.Count((part) => part is not null);

        /// <summary>
        /// Constructs a Tower with Empty Parts
        /// </summary>
        /// <param name="position">Tower's Initial Position</param>
        /// <param name="size">Tower's Drawing Size</param>
        /// <param name="texture">Tower's Texture</param>
        public Tower(Vector2 position, Vector2 size, Texture2D texture) : base(position, size, texture)
        {
            _attachedParts = new PartDefinition[4];
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Gets a part from an <paramref name="index"/>. If <paramref name="shiftIfNull"/> is <see cref="true"/>, then the method will attempt to choose another attached part
        /// </summary>
        /// <param name="index">Index of the part</param>
        /// <param name="shiftIfNull">Attempts to shift to another part if the part at the index is null</param>
        /// <returns></returns>
        public PartDefinition? GetPartFromIndex(int index, bool shiftIfNull)
        {
            PartDefinition part = _attachedParts[index];
            if(shiftIfNull && part is null)
            {
                foreach (PartDefinition otherPart in _attachedParts)
                {
                    if (otherPart is not null)
                    {
                        part = otherPart;
                        break;
                    }
                }
            }
            return part;
        }

        public virtual void SetPart(PartDefinition part, Slot slot)
        {
            switch (slot)
            {
                case Slot.First:
                    _attachedParts[0] = part;
                    break;
                case Slot.Second:
                    _attachedParts[1] = part;
                    break;
                case Slot.Third:
                    _attachedParts[2] = part;
                    break;
                case Slot.Fourth:
                    _attachedParts[3] = part;
                    break;
            }
        }
    }
}
